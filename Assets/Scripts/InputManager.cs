using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [Header("Swipe Stuff")]
    public float swipeAngle = 0;
    public float swipeResist = 1f;

    public bool isDragging = false;
    private Dot selectedDot;

    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;
    private Vector2 moveDirection = Vector2.zero;

    private Board board;


    void Start()
    {


        board = FindObjectOfType<Board>();

    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono);
            if (Physics.Raycast(ray, out var hit, float.MaxValue, LayerMask.GetMask("Dot")))
            {
                if (hit.transform.TryGetComponent<Dot>(out var dot))
                {
                    isDragging = true;
                    board.Dragging();
                    this.selectedDot = dot;
                    if (board.currentState == GameState.move)
                    {
                        firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    }
                }
            }
        }

        if (isDragging && Input.GetMouseButton(0))
        {
            Dragging();
        }
        if (isDragging && Input.GetMouseButtonUp(0))
        {
            EndDrag();
        }
    }

    void Dragging()
    {
        if (board.currentState == GameState.move)
        {
            finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Move2();
        }
    }

    void EndDrag()
    {
        isDragging = false;
        moveDirection = Vector2.zero;
        board.currentState = GameState.wait;
        board.RelocationAll();
        board.DestroyMatches();
    }

    private Vector2 GetDirection()
    {

        swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;


        if (swipeAngle > -45 && swipeAngle <= 45)
        {

            return Vector2.right;
        }
        else if (swipeAngle > 45 && swipeAngle <= 135)
        {

            return Vector2.up;
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135))
        {

            return Vector2.left;
        }
        else if (swipeAngle < -45 && swipeAngle >= -135)
        {
            return Vector2.down;
        }
        return Vector2.zero;
    }
    void Move2()
    {

        var distance = Vector2.Distance(firstTouchPosition, finalTouchPosition);
        moveDirection = GetDirection();
        if (moveDirection.sqrMagnitude < swipeResist) return;

        if (moveDirection == Vector2.right || moveDirection == Vector2.left) // move horizontal
        {
            for (int i = 0; i < board.width; i++)
            {

                var dot = board.allDots[i, selectedDot.row].GetComponent<Dot>();

                var newPos = (i + distance * moveDirection.x) % board.width;

                if (newPos < 0)
                {
                    newPos += board.width;
                }
                dot.transform.position = new Vector3(newPos, dot.GetComponent<Dot>().row, dot.transform.position.z);
            }

        }
        else // move veritacal
        {
            for (int i = 0; i < board.height; i++)
            {
                var dot = board.allDots[selectedDot.column, i].GetComponent<Dot>();
                var newPos = (i + distance * moveDirection.y) % board.height;
                if (newPos < 0)
                {
                    newPos += board.height;
                }
                dot.transform.position = new Vector3(dot.GetComponent<Dot>().column, newPos, dot.transform.position.z);

            }

        }

    }
}
