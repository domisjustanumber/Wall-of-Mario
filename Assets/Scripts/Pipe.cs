using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pipe : MonoBehaviour
{
    public Transform connection;
    [SerializeField] private InputActionReference enterPipeAction;
    public Vector3 enterDirection = Vector3.down;
    public Vector3 exitDirection = Vector3.zero;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (connection != null && other.CompareTag("Player") && other.TryGetComponent(out Player player))
        {
            bool enterByKey = enterPipeAction != null && enterPipeAction.action.IsPressed();
            bool enterByMovement = false;
            if (Mathf.Abs(enterDirection.x) > 0.5f && enterPipeAction != null)
            {
                var moveAction = enterPipeAction.action.actionMap?.FindAction("Move");
                if (moveAction != null)
                {
                    float moveX = moveAction.ReadValue<Vector2>().x;
                    enterByMovement = (enterDirection.x > 0f && moveX > 0.25f) || (enterDirection.x < 0f && moveX < -0.25f);
                }
            }
            if (enterByKey || enterByMovement)
                StartCoroutine(Enter(player));
        }
    }

    private IEnumerator Enter(Player player)
    {
        player.movement.enabled = false;

        Vector3 enteredPosition = transform.position + enterDirection;
        Vector3 enteredScale = Vector3.one * 0.5f;

        yield return Move(player.transform, enteredPosition, enteredScale);
        yield return new WaitForSeconds(1f);

        var sideSrolling = Camera.main.GetComponent<SideScrollingCamera>();
        bool goingUnderground = connection.position.y < sideSrolling.undergroundThreshold;

        if (goingUnderground)
        {
            yield return ScreenFade.Instance.FadeToBlack(0.3f);
            sideSrolling.SetUnderground(true);
        }

        if (exitDirection != Vector3.zero)
        {
            player.transform.position = connection.position - exitDirection;
            yield return Move(player.transform, connection.position + exitDirection, Vector3.one);
        }
        else
        {
            player.transform.position = connection.position;
            player.transform.localScale = Vector3.one;
        }

        if (goingUnderground)
            yield return ScreenFade.Instance.FadeFromBlack(0.3f);
        else
            yield return sideSrolling.ScrollToSurface(1f);

        player.movement.enabled = true;
    }

    private IEnumerator Move(Transform player, Vector3 endPosition, Vector3 endScale)
    {
        float elapsed = 0f;
        float duration = 1f;

        Vector3 startPosition = player.position;
        Vector3 startScale = player.localScale;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            player.position = Vector3.Lerp(startPosition, endPosition, t);
            player.localScale = Vector3.Lerp(startScale, endScale, t);
            elapsed += Time.deltaTime;

            yield return null;
        }

        player.position = endPosition;
        player.localScale = endScale;
    }

}
