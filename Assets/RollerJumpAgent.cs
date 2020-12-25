using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;


public class RollerJumpAgent : Agent
{

    Rigidbody rbody;
    [SerializeField]
    Transform target;
    [SerializeField]
    Transform obstacle;
    [SerializeField]
    [Range(1, 10)]
    float forceMultiplier = 1;

    readonly Vector3 defaultPosition = new Vector3(-4.23f, 0.5f, 0);

    // Start is called before the first frame update
    void Start()
    {
        rbody = this.GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        if(this.transform.localPosition.y < 0)
        {
            this.rbody.angularVelocity = Vector3.zero;
            this.rbody.velocity = Vector3.zero;
            this.transform.localPosition = defaultPosition;
        }


        target.localPosition = new Vector3(Mathf.Min(4, Random.value * 8 - 4), 0.5f, Mathf.Min(4,Random.value * 8 - 4));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(target.localPosition);
        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(this.rbody.velocity);

    }

    public override void OnActionReceived(float[] vectorAction)
    {
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.y = vectorAction[1];
        controlSignal.z = vectorAction[2];
        this.rbody.AddForce(controlSignal * forceMultiplier);

        //Rewards
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, target.localPosition);

        if(distanceToTarget < 1.42f)
        {
            SetReward(1.0f);
            EndEpisode();
        }

        else if(this.transform.localPosition.y < 0)
        {
            EndEpisode();
        }

        else if(Vector3.Distance(this.transform.localPosition, obstacle.transform.localPosition) < 1)
        {
            SetReward(-1.0f);
            EndEpisode();
        }
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetAxis("Horizontal");
        actionsOut[1] = Input.GetKeyDown(KeyCode.Space)? -2 : 0;
        actionsOut[2] = Input.GetAxis("Vertical");
    }


}
