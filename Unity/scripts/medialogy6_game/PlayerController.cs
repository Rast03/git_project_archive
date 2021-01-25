using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float distance = 1f;

    public AudioSource walkingSound;
    public AudioSource pushSound;
    public AudioSource rotateWoodSound;
    public AudioSource rotateKrystalSound;

    public Transform movePoint;
    public LayerMask stopsMovement;
    public LayerMask interactive;
    public LayerMask boxMask;
    public Animator animator;
    GameObject box;
    Stack<Vector3> state = new Stack<Vector3>();
    GameObject lastInteracted = null;

    void Start()
    {
        movePoint.parent = null;
    }

    void Update()
    {

        Physics2D.queriesStartInColliders = false;
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, speed * Time.deltaTime);
        RaycastHit2D upRay = Physics2D.Raycast(transform.position, Vector2.up * transform.localScale.y, distance, boxMask);
        RaycastHit2D downRay = Physics2D.Raycast(transform.position, Vector2.down * transform.localScale.y, distance, boxMask);
        RaycastHit2D leftRay = Physics2D.Raycast(transform.position, Vector2.left * transform.localScale.x, distance, boxMask);
        RaycastHit2D rightRay = Physics2D.Raycast(transform.position, Vector2.right * transform.localScale.x, distance, boxMask);

        if (Input.GetKeyDown(KeyCode.R) && rightRay.collider != null && rightRay.collider.gameObject.tag.Contains("Rotatable"))
        {
            this.rotateBox(rightRay);
        }

        if (Input.GetKeyDown(KeyCode.R) && leftRay.collider != null && leftRay.collider.gameObject.tag.Contains("Rotatable"))
        {
            this.rotateBox(leftRay);
        }

        if (Input.GetKeyDown(KeyCode.R) && upRay.collider != null && upRay.collider.gameObject.tag.Contains("Rotatable"))
        {
            this.rotateBox(upRay);
        }

        if (Input.GetKeyDown(KeyCode.R) && downRay.collider != null && downRay.collider.gameObject.tag.Contains("Rotatable"))
        {
            this.rotateBox(downRay);
        }

        if (Vector3.Distance(transform.position, movePoint.position) <= .05f)
        {
            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f)
            {
                Vector3 xPos = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);
                if (!Physics2D.OverlapCircle(movePoint.position + xPos, .2f, stopsMovement))
                {
                    movePoint.position += xPos;
                    this.state.Push(xPos);

                    if (Input.GetAxisRaw("Horizontal") == 1 && rightRay.collider != null && rightRay.collider.gameObject.tag.Contains("Pushable"))
                    {
                        Vector3 xPos_Right = rightRay.collider.gameObject.transform.position + xPos;
                        if (!Physics2D.OverlapCircle(xPos_Right, .2f, stopsMovement) && !Physics2D.OverlapCircle(xPos_Right, .2f, interactive))
                        {
                            this.pushBox(rightRay, xPos);
                        }
                        else
                        {
                            movePoint.position -= xPos;
                        }
                    }
                    else if (Input.GetAxisRaw("Horizontal") == -1 && leftRay.collider != null && leftRay.collider.gameObject.tag.Contains("Pushable"))
                    {
                        Vector3 yPos_Left = leftRay.collider.gameObject.transform.position + xPos;
                        if (!Physics2D.OverlapCircle(yPos_Left, .2f, stopsMovement) && !Physics2D.OverlapCircle(yPos_Left, .2f, interactive))
                        {
                            this.pushBox(leftRay, xPos);
                        }
                        else
                        {
                            movePoint.position -= xPos;
                        }
                    }
                }
            }
            else if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f)
            {

                Vector3 yPos = new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f);

                if (!Physics2D.OverlapCircle(movePoint.position + yPos, .2f, stopsMovement))
                {
                    movePoint.position += yPos;
                    this.state.Push(yPos);

                    if (Input.GetAxisRaw("Vertical") == 1 && upRay.collider != null && upRay.collider.gameObject.tag.Contains("Pushable"))
                    {
                        Vector3 yPos_Up = upRay.collider.gameObject.transform.position + yPos;
                        if (!Physics2D.OverlapCircle(yPos_Up, .2f, stopsMovement) && !Physics2D.OverlapCircle(yPos_Up, .2f, interactive))
                        {
                            this.pushBox(upRay, yPos);
                        }
                        else
                        {
                            movePoint.position -= yPos;
                        }
                    }
                    else if (Input.GetAxisRaw("Vertical") == -1 && downRay.collider != null && downRay.collider.gameObject.tag.Contains("Pushable"))
                    {
                        Vector3 yPos_Down = downRay.collider.gameObject.transform.position + yPos;
                        if (!Physics2D.OverlapCircle(yPos_Down, .2f, stopsMovement) && !Physics2D.OverlapCircle(yPos_Down, .2f, interactive))
                        {
                            this.pushBox(downRay, yPos);
                        }
                        else
                        {
                            movePoint.position -= yPos;
                        }
                    }
                }
            }

            animator.SetBool("moving", false);
            walkingSound.Play();

            return;
        }

        animator.SetBool("moving", true);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + Vector2.up * transform.localScale.y * distance);
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + Vector2.down * transform.localScale.y * distance);
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + Vector2.right * transform.localScale.x * distance);
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + Vector2.left * transform.localScale.x * distance);
    }

    void pushBox(RaycastHit2D ray, Vector3 position)
    {
        box = ray.collider.gameObject;
        lastInteracted = box;
        Vector3 newPosition = box.transform.position + position;
        StateManager stateManager = (StateManager)box.GetComponent(typeof(StateManager));

        pushSound.Play();
        stateManager.pushNewState(box.transform.position);
        box.GetComponent<pushingScript>().movePoint = newPosition;
    }

    void rotateBox(RaycastHit2D ray)
    {
        if (ray.transform.tag.Contains("Reflective"))
        {
            rotateWoodSound.Play();
            ray.transform.GetComponent<mirrorController>().newPosition = true;
            switch (ray.transform.GetComponent<mirrorController>().corner)
            {
                case 1:
                    ray.transform.GetComponent<mirrorController>().corner = 2;
                    ray.transform.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Mirror_rot2");
                    break;
                case 2:
                    ray.transform.GetComponent<mirrorController>().corner = 3;
                    ray.transform.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Mirror_rot3");
                    break;
                case 3:
                    ray.transform.GetComponent<mirrorController>().corner = 4;
                    ray.transform.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Mirror_rot4");
                    break;
                case 4:
                    ray.transform.GetComponent<mirrorController>().corner = 1;
                    ray.transform.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Mirror_rot1");
                    break;
            }
        }
        else if (ray.transform.tag.Contains("Prism"))
        {
            rotateKrystalSound.Play();
            ray.transform.GetComponent<prismController>().newPosition = true;
            switch (ray.transform.GetComponent<prismController>().direction)
            {
                case "down":
                    ray.transform.GetComponent<prismController>().direction = "left";
                    ray.transform.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Prism_rot_1");
                    break;
                case "up":
                    ray.transform.GetComponent<prismController>().direction = "right";
                    ray.transform.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Prism_rot_3");
                    break;
                case "left":
                    ray.transform.GetComponent<prismController>().direction = "up";
                    ray.transform.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Prism_rot_2");
                    break;
                case "right":
                    ray.transform.GetComponent<prismController>().direction = "down";
                    ray.transform.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Prism_rot_4");
                    break;
            }
        }
    }

    public void undoMove()
    {
        if (lastInteracted != null)
        {
            StateManager stateManager = (StateManager)lastInteracted.GetComponent(typeof(StateManager));
            if (!stateManager.isStateEmpty())
            {
                Vector3 lastInteractedState = stateManager.getLastState();
                pushSound.Play();
                box.GetComponent<pushingScript>().movePoint = lastInteractedState;

                if (this.state.Count > 0)
                {
                    Vector3 playerLastState = this.state.Pop();
                    movePoint.position -= playerLastState;
                }
            }
        }
    }
}
