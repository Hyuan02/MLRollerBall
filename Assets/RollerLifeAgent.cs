using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;


public class RollerLifeAgent : Agent
{

    Rigidbody rbody;
    [SerializeField]
    [Range(1, 10)]
    float forceMultiplier = 1;

    [SerializeField]
    Vector3 defaultPosition = new Vector3(26.5f, 0.6f, 22.7f);

    [SerializeField]
    private float totalLife = 100f;

    [SerializeField]
    private ItensSpawner spawner;
    

    int currentRedPills = ItensSpawner.NUMBER_OF_RED_PILLS;
    int currentGreenPills = ItensSpawner.NUMBER_OF_GREEN_PILLS;

    // Start is called before the first frame update
    void Start()
    {
        rbody = this.GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {

        this.rbody.angularVelocity = Vector3.zero;
        this.rbody.velocity = Vector3.zero;
        this.transform.localPosition = new Vector3(Random.Range(18,34), 0.6f, Random.Range(18,34));
        totalLife = 100f;
        spawner.SpawnItems();
        currentGreenPills = ItensSpawner.NUMBER_OF_GREEN_PILLS;
        currentRedPills = ItensSpawner.NUMBER_OF_RED_PILLS;


    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(totalLife);
        sensor.AddObservation(this.transform.localPosition);
        var velocity = transform.InverseTransformDirection(rbody.velocity);
        sensor.AddObservation(velocity.x);
        sensor.AddObservation(velocity.z);
        sensor.AddObservation(currentGreenPills);
        sensor.AddObservation(currentRedPills);

    }

    public override void OnActionReceived(float[] vectorAction)
    {
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = vectorAction[1];
        this.rbody.AddForce(controlSignal * forceMultiplier);

        totalLife -= 0.18f;
        if(totalLife < 0)
        {
            SetReward(-10.0f);
            EndEpisode();
        }

        //if(this.transform.position.y < -1)
        //{
        //    SetReward(-10f);
        //    EndEpisode();
        //}

    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetAxis("Horizontal");
        actionsOut[1] = Input.GetAxis("Vertical");
    }

    public void GetRedPill()
    {
        currentRedPills -= 1;
        totalLife -= 10;
        AddReward(-10f);
        if(currentRedPills < 1)
        {
            SetReward(-10f);
            EndEpisode();
        }
    }

    public void GetGreenPill()
    {
        currentGreenPills -= 1;
        AddReward(10f);
        totalLife += 10;
        if(currentGreenPills < 1)
        {
            SetReward(15.0f);
            EndEpisode();
        }
    }
}
