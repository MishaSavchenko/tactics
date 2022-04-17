using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class KeypadPath : MonoBehaviour
{
    private Map map_;

    void Start()
    {
        line = this.gameObject.AddComponent<LineRenderer>();
        map_ = GameObject.Find("field").GetComponent<Map>();
    }

    GameObject start_tile = null;
    GameObject goal_tile = null;
    
    GameObject last_tile = null;
    GameObject current_tile = null;

    List<Vector3> path = new List<Vector3>(); 
    List<string> path_names = new List<string>();
    List<string> last_path_names = new List<string>();
    
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit ))
            {
                if(start_tile == null && goal_tile == null)
                {
                    start_tile = hit.collider.gameObject;
                    Debug.Log("Start tile : " + start_tile.name);
                }
                else if(start_tile != null && goal_tile == null)
                {
                    goal_tile = hit.collider.gameObject;
                    Debug.Log("Goal tile : " + goal_tile.name);
                }
                else
                {
                    Debug.Log("reseting");
                    start_tile = null;
                    goal_tile = null;
                    last_tile = null;
                }
            }
        }


        if(start_tile != null && goal_tile == null)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit ))
            {
                GameObject new_tile = hit.collider.gameObject;

                if(map_.IsTileOccupied(new_tile.name))
                {
                    last_path_names = new List<string>(path_names);
                    path_names = map_.GetPath(start_tile.name, new_tile.name);
                    path = map_.TilesToPositions(path_names);

                    if(last_tile != new_tile)
                    {
                        last_tile = new_tile;
                        if (path.Count != 0 )
                        {
                            for(int g = 0; g < created_arrows.Count; g++)
                            {
                                Destroy(created_arrows[g]);
                            }
                            ConstructArrow(path);
                        }
                    }
                }
            }
        }
    }

    LineRenderer line;
    public void ConstructDebugArrow(List<Vector3> arrow_path)
    {
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
        line.positionCount = path.Count;
        Vector3 line_offset = new Vector3(0.0f,0.15f,0.0f);
        for(int i=0; i < arrow_path.Count; i++)
        {   
            Vector3 new_position = path[i];
            line.SetPosition(i, new_position + line_offset); 
        }
    }

    public GameObject arrow_goal;
    public GameObject arrow_line;
    public GameObject arrow_turn_left;
    private List<GameObject> created_arrows = new List<GameObject>();
    public Material arrow_material;

    public void ConstructArrow(List<Vector3> arrow_path)
    {
        Vector3 line_offset = new Vector3(0.0f,0.15f,0.0f);

        Vector3 final_position = arrow_path[arrow_path.Count - 1];
        Quaternion goal_arrow_rot = new Quaternion();
        if(arrow_path.Count >=2)
        {
            Vector3 last_to_final_position = arrow_path[arrow_path.Count - 2];
            goal_arrow_rot.SetLookRotation(final_position - last_to_final_position); 
        }
        GameObject go_final = Instantiate(arrow_goal, final_position + line_offset, goal_arrow_rot);
        go_final.transform.parent = this.transform;
        go_final.GetComponent<MeshRenderer>().material = arrow_material;
        
        created_arrows.Add(go_final);
        GameObject last_game_object  = go_final;

        for(int i=arrow_path.Count - 2; i > 0; i--)
        {   
            Vector3 last_position = arrow_path[i + 1];
            Vector3 new_position = arrow_path[i];
            Vector3 next_position = arrow_path[i - 1];

            Vector3 prev_diff = last_position - new_position;
            Vector3 next_diff = next_position - new_position;
            
            Vector3 inter_diff = next_diff + prev_diff;

            Quaternion last_rot = Quaternion.identity;
            Quaternion rot = Quaternion.identity;
            last_rot.SetLookRotation(prev_diff);
    
            GameObject arrow_type = null;
            if (inter_diff[0] == 0.0 || inter_diff[2] == 0.0)
            {
                arrow_type = arrow_line;
                rot = last_rot;
            }   
            else
            {
                arrow_type = arrow_turn_left;
                // down + right  ->  1.0  0.0 -1.0
                if(inter_diff == Vector3.right + Vector3.back )
                {
                    rot = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                }
                // down + left   -> -1.0  0.0 -1.0
                else if (inter_diff == Vector3.left + Vector3.back )
                {
                    rot = Quaternion.Euler(0.0f, 90.0f, 0.0f);
                }
                // up + left     -> -1.0  0.0  1.0
                else if (inter_diff == Vector3.forward + Vector3.left)
                {
                    rot = Quaternion.Euler(0.0f, 180.0f, 0.0f);
                }
                // up + right    ->  1.0  0.0  1.0
                else if (inter_diff == Vector3.forward + Vector3.right)
                {       
                    rot = Quaternion.Euler(0.0f, 270.0f, 0.0f);
                }
                else
                {
                    Debug.LogWarning("Corner arrow rotation math failed");
                }
            }

            GameObject go = Instantiate(arrow_type, new_position  + line_offset, rot);
            go.transform.parent = this.transform;
            go.GetComponent<MeshRenderer>().material = arrow_material;
            last_game_object = go;
            created_arrows.Add(go);
        }
    }
}
