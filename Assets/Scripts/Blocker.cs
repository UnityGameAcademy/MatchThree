using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blocker : MonoBehaviour
{

    // x and y index used for determining position in the Board's array
    public int xIndex;
    public int yIndex;
    public Board board;

    public float delayTime = 0.5f;
    public float scaleTime = 0.3f;

    // initialze the Blockers's array index and cache a reference to the Board
    public void Init(int x, int y, Board board)
    {
        if (board == null)
            return;

        xIndex = x;
        yIndex = y;
        board.allBlockers[x, y] = this;
        ScaleUp();
    }

    // 
    public void ScaleDown()
    {
        StartCoroutine(ScaleRoutine(Vector3.one, Vector3.zero, delayTime, scaleTime));
    }

    public void SelfDestruct()
    {
        StartCoroutine(SelfDestructRoutine());
    }

    IEnumerator SelfDestructRoutine()
    {
        ScaleDown();
        yield return new WaitForSeconds(delayTime);
        Destroy(gameObject, delayTime);

    }

    public void ScaleUp()
    {
        // scale the game piece
        StartCoroutine(ScaleRoutine(Vector3.zero, Vector3.one));

        // add any extra effects/particles/sounds here
    }


    // coroutine to handle initial animation to grow the blocker
    IEnumerator ScaleRoutine(Vector3 startScale, Vector3 destinationScale, float delay = 0.5f, float timeToScale = 0.3f)
    {
        // set our starting scale
        transform.localScale = startScale;

        // wait for delay
        yield return new WaitForSeconds(delay);

        // are we done growing?
        bool isComplete = false;

        // how much time has passed since we started growing
        float elapsedTime = 0f;

        // while we have not reached the destination, check to see if we are close enough
        while (!isComplete)
        {
            // are we close to a local scale of (1,1,1)
            if (Vector3.Distance(transform.localScale, destinationScale) < 0.01f)
            {
                // we have reached the destination
                isComplete = true;

                transform.localScale = destinationScale;
                break;
            }

            // increment the total running time by the Time elapsed for this frame
            elapsedTime += Time.deltaTime;

            // calculate the Lerp value
            float t = Mathf.Clamp(elapsedTime / timeToScale, 0f, 1f);

            // use a smooth step function to interpolate
            t = t * t * t * (t * (t * 6 - 15) + 10);


            // move the game piece
            transform.localScale = Vector3.Lerp(startScale, destinationScale, t);

            // wait until next frame
            yield return null;
        }
    }

}
