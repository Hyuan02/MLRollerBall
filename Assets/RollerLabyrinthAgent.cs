using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine.AI;

public class RollerLabyrinthAgent : Agent
{

    Rigidbody rbody;
    NavMeshAgent nvAgent;
    [SerializeField]
    Transform target;
    [SerializeField]
    [Range(1, 10)]
    float forceMultiplier = 1;


    float actualPathDistance;

    [SerializeField]
    Vector3 defaultRollerPosition = new Vector3(26.5f, 0.6f, 22.7f);

    readonly Vector3 defaultTargetPosition = new Vector3(82.2f, 0.6f, 27.5f);

    // Start is called before the first frame update
    void Start()
    {

        rbody = this.GetComponent<Rigidbody>();
        nvAgent = this.GetComponent<NavMeshAgent>();
    }

    public override void OnEpisodeBegin()
    {

        this.rbody.angularVelocity = Vector3.zero;
        this.rbody.velocity = Vector3.zero;
        this.transform.localPosition = defaultRollerPosition;
        target.localPosition = defaultTargetPosition;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //sensor.AddObservation(target.position);
        //sensor.AddObservation(this.transform.position);
        sensor.AddObservation(this.rbody.velocity.x);
        sensor.AddObservation(this.rbody.velocity.z);
        sensor.AddObservation(actualPathDistance);
        //sensor.AddObservation((this.target.position - this.transform.position).normalized);
    }

    public override void OnActionReceived(float[] vectorAction)
    {



        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = vectorAction[1];
        this.rbody.AddForce(controlSignal * forceMultiplier);

        //Vector3 vectorToGo;
        //vectorToGo.x = vectorAction[0];
        //vectorToGo.y = vectorAction[1];
        //vectorToGo.z = vectorAction[2];

        //nvAgent.SetDestination(vectorToGo);

        NavMeshPath path = new NavMeshPath();
        bool result = NavMesh.CalculatePath(this.transform.position, target.transform.position, NavMesh.AllAreas, path);

        print("The path is: " + result);
        if (result)
        {
            actualPathDistance = GetPathLength(path);
            print("Distance: " + actualPathDistance);
        }


        //Rewards
        //float distanceToTarget = Vector3.Distance(this.transform.localPosition, target.localPosition);

        if(actualPathDistance < 5f)
        {
            SetReward(1.0f);
            EndEpisode();
        }

        else if(this.transform.localPosition.y < 0)
        {
            EndEpisode();
        }
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetAxis("Horizontal");

        actionsOut[1] = Input.GetAxis("Vertical");

    }


    public static float GetPathLength(NavMeshPath path)
    {
        float lng = 0.0f;

        if ((path.status != NavMeshPathStatus.PathInvalid))
        {
            for (int i = 1; i < path.corners.Length; ++i)
            {
                lng += Vector3.Distance(path.corners[i - 1], path.corners[i]);
            }
        }

        return lng;
    }
}
