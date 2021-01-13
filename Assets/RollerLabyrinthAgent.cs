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
    [Range(1, 100)]
    float forceMultiplier = 1;


    float actualPathDistance;
    float previousPathDistance;

    Collider[] hitWallColliders;

    [SerializeField]
    Vector3 defaultRollerPosition = new Vector3(26.5f, 0.6f, 22.7f);

    readonly Vector3 defaultTargetPosition = new Vector3(82.2f, 2.08f, 27.5f);

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
        sensor.AddObservation(this.transform.position);
        sensor.AddObservation(this.rbody.velocity.x);
        sensor.AddObservation(this.rbody.velocity.z);
        sensor.AddObservation(actualPathDistance);
        sensor.AddObservation((this.target.position - this.transform.position).normalized);
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

        //print("The path is: " + result);
        if (result)
        {

            actualPathDistance = GetPathLength(path);

            if(previousPathDistance != 0)
            {
                if(actualPathDistance < previousPathDistance)
                {
                    AddReward(0.01f);
                }
                else
                {
                    AddReward(-0.1f);
                }
            }
            previousPathDistance = actualPathDistance;
            //print("Distance: " + actualPathDistance);
        }

        hitWallColliders = new Collider[1];
        Physics.OverlapSphereNonAlloc(this.transform.position, 2f, hitWallColliders);

        foreach(Collider wall in hitWallColliders)
        {
            if (wall.CompareTag("Walls"))
            {
                AddReward(-0.1f);
                
            }
        }

        //Rewards
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, target.localPosition);

        if (distanceToTarget < 5f)
        {
            SetReward(1000.0f);
            Debug.Log("Founded");
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
