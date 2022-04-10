using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentHandler : MonoBehaviour
{
    public float height = 0.1f;
    public int interpolationFramesCount = 45;
    public float rotation_speed = 3.0f;
    int elapsedFrames = 0;

    Vector3 start_position;
    Vector3 goal_position;

    private GameObject primary_agent;

    List<Vector3> latest_path = null;
    List<Vector3>.Enumerator path_em = new List<Vector3>().GetEnumerator();

    [Header("Stats")]
    public string code_name;
    public string team_name;
    public double health = 100;
    public double max_health = 100;
    public double speed = 5; 
    public double strength = 5;
    public double inteligence = 5;
    public double range = 5;


    private GameObject agent_interface; 
    private GameObject player_view; 
    private Camera primary_camera;
    private GameObject health_bar;

    public List<GameObject> GetGameObjectsFromDirectory(string directory_path)
    {
        var agents = Resources.LoadAll(directory_path, typeof(GameObject));
        List<GameObject> loaded_agents = new List<GameObject>();
        foreach(var agent in agents)
        {
            loaded_agents.Add((GameObject)agent);
        }
        return loaded_agents;
    }

    void Start()
    {
        primary_agent = this.gameObject;

        start_position = Vector3.zero;
        goal_position = Vector3.zero;

        player_view = GameObject.Find("Main Camera").gameObject;
        primary_camera = player_view.GetComponent<Camera>();   

        health_bar = this.transform.Find("HealthBar").gameObject;

        agent_interface = this.transform.Find("AgentInterface").gameObject;
        agent_interface.SetActive(false);

    }

    Vector3 QuadraticBezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        return (1-t) * (1-t) * p0 + 2.0f*(1.0f-t)*t*p1 + t * t * p2;
    }

    public void GeneratePathTrajectory(List<Vector3> path)
    {
        latest_path = path; 
        path_em = latest_path.GetEnumerator();
        path_em.MoveNext();
        start_position = path_em.Current;
        path_em.MoveNext();
        goal_position = path_em.Current;
    }

    void Update()
    {   
        // rotate ui and health bar
        agent_interface.transform.rotation = primary_camera.transform.rotation;
        health_bar.transform.rotation = primary_camera.transform.rotation;

        if (latest_path != null)
        {
            float interpolation_ratio = (float)elapsedFrames / interpolationFramesCount;
            elapsedFrames = (elapsedFrames + 1) % (interpolationFramesCount + 1);

            Vector3 mid_point = (goal_position - start_position) * 0.5f + start_position + new Vector3(0.0f, height, 0.0f);
            Vector3 inter = QuadraticBezierCurve(start_position, mid_point, goal_position, interpolation_ratio);
            // Target to look at 
            Vector3 look_toward_vector = goal_position - primary_agent.transform.position;
            look_toward_vector.y = 0.0f;
            // Goal orientation of the agent
            Quaternion goal_rotation = Quaternion.LookRotation(look_toward_vector, Vector3.up);
            // Interpolation between current rotation and goal rotation
            primary_agent.transform.rotation = Quaternion.Slerp(primary_agent.transform.rotation, 
                                                                goal_rotation, 
                                                                interpolation_ratio * rotation_speed);
            primary_agent.transform.position = inter;
            
            if(elapsedFrames == 0 )
            {
                start_position = goal_position; 
                if(path_em.MoveNext())
                {
                    goal_position = path_em.Current;
                }
                else
                {
                    latest_path = null;
                    path_em.Dispose();
                }
            }
        }   
    }


}
