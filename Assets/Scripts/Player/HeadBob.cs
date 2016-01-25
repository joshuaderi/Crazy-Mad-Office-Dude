//Class to make first person camera bob gently up and down while walking
using UnityEngine;

public class HeadBob : MonoBehaviour
{
    //Strength of head bob - amplitude of sine wave
    public float Strength = 1.0f;

    //Frequency of wave
    public float BobAmount = 2.0f;

    //Neutral head height position
    public float HeadY = 1.0f;

    //Cached transform
    private Transform thisTransform;

    //Elapsed Time since movement
    private float elapsedTime = 0.0f;

    void Start()
    {
        //Get transform
        thisTransform = transform;
    }

    // Update is called once per frame
    void Update()
    {
        //If input is not allowed, then exit
        if (!GameManager.Instance.InputAllowed) return;

        float yPos = HeadY;
        float totalMovement = GetTotalMovement();
        if (totalMovement > 0.0f) yPos += GetOffsetY(totalMovement);
        else elapsedTime = 0.0f;
        
        thisTransform.position = new Vector3(
            thisTransform.position.x,
            yPos,
            thisTransform.position.z);
    }

    private float GetOffsetY(float totalMovement)
    {
        elapsedTime += Time.deltaTime;
        return Mathf.Sin(elapsedTime * BobAmount) * Strength * totalMovement;
    }

    private static float GetTotalMovement()
    {
        float horizontal = Mathf.Abs(Input.GetAxis("Horizontal"));
        float vertical = Mathf.Abs(Input.GetAxis("Vertical"));

        return Mathf.Clamp(horizontal + vertical, 0.0f, 1.0f);
    }
}