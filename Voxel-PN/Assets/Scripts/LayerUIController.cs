using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;

public class LayerUIController : MonoBehaviour
{

	public GameObject voxel_button_prefab;
	public GameObject voxel_prefab_odd, voxel_prefab_even;
	public GameObject background_panel;
	public GameObject main_canvas;
	public GameObject voxel_holder;
    public GameObject even_button_holder, odd_button_holder;
	public float button_spacing;
	public int num_buttons;

	private int[,] layer, zero_layer;
	private List<int[,]> all_layers;
	private Button[,] buttons;
	private int cur_layer;

    // Start is called before the first frame update
    void Start()
    {


    	cur_layer = 0;
    	layer = new int[num_buttons*2 - 1, num_buttons*2 - 1];
    	buttons = new Button[num_buttons*2 - 1, num_buttons*2 - 1];
    	all_layers = new List<int[,]>();

    	//float width = background_panel.GetComponent<RectTransform>().sizeDelta[0];
    	float bk_dim = background_panel.GetComponent<RectTransform>().sizeDelta[1];

    	//float vb_dim = voxel_button_prefab.GetComponent<RectTransform>().sizeDelta[0];
    	//float vb_height = voxel_button_prefab.GetComponent<RectTransform>().sizeDelta[1];

    	float button_dim = ((bk_dim - button_spacing) / num_buttons) - button_spacing;
    	//print(button_dim);
    	//int num_buttons = (int) ((width - button_spacing) / (vb_width + button_spacing));
    	//int y_buttons = (int) (height / (vb_height + button_spacing));

    	//vb_width = 
    	//vb_width = (width - button_spacing) / (vb_width + button_spacing);
        
        for(int x = 0; x < num_buttons*2 - 1; x++) {
            for(int y = 0; y < num_buttons*2 - 1; y++) {
                layer[x,y] = -1;
            }
        }

        for(int x = 0; x < num_buttons; x++) {
        	for(int y = 0; y < num_buttons; y++) {
        		GameObject g = Instantiate(voxel_button_prefab, new Vector3(0, 0, 0), Quaternion.identity);
        		g.GetComponent<RectTransform>().parent = even_button_holder.transform;
        		g.GetComponent<RectTransform>().localPosition = new Vector3(x * (button_dim + button_spacing) - bk_dim/2 + button_dim/2 + button_spacing, y * (button_dim + button_spacing) - bk_dim/2 + button_dim/2 + button_spacing, 0);
        		g.transform.localScale = new Vector3(1,1,1);
        		g.GetComponent<RectTransform>().localEulerAngles = new Vector3(0,0,0);

        		ColorBlock colors = g.GetComponent<Button>().colors;
         		colors.normalColor = Color.white;
         		colors.highlightedColor = new Color32(0, 100, 200, 255);
         		g.GetComponent<Button>().colors = colors;
        		//g.GetComponent<Button>().onClick.AddListener(addVoxel);
        		int tx = x*2;
        		int ty = y*2;
        		buttons[x*2, y*2]  = g.GetComponent<Button>();
        		g.GetComponent<Button>().onClick.AddListener(() => { addVoxel(tx,ty); });
        		layer[x*2, y*2] = -1;
                //layer[x*2, y*2 + 1] = 0;

        	}
        }

        for(int x = 0; x < num_buttons - 1; x++) {
            for(int y = 0; y < num_buttons - 1; y++) {
                GameObject g = Instantiate(voxel_button_prefab, new Vector3(0, 0, 0), Quaternion.identity);
                g.GetComponent<RectTransform>().parent = odd_button_holder.transform;
                g.GetComponent<RectTransform>().localPosition = new Vector3(x * (button_dim + button_spacing) - bk_dim/2 + button_dim/2 + button_spacing + button_dim/2, y * (button_dim + button_spacing) - bk_dim/2 + button_dim/2 + button_spacing + button_dim/2, 0);
                g.transform.localScale = new Vector3(1,1,1);
                g.GetComponent<RectTransform>().localEulerAngles = new Vector3(0,0,0);

                ColorBlock colors = g.GetComponent<Button>().colors;
                colors.normalColor = Color.white;
                colors.highlightedColor = new Color32(0, 100, 200, 255);
                g.GetComponent<Button>().colors = colors;
                //g.GetComponent<Button>().onClick.AddListener(addVoxel);
                int tx = x*2 + 1;
                int ty = y*2 + 1;
                buttons[x*2 + 1, y*2 + 1]  = g.GetComponent<Button>();
                g.GetComponent<Button>().onClick.AddListener(() => { addVoxel(tx,ty); });
                layer[x*2 + 1, y*2 + 1] = -1;
            }
        }

        zero_layer = layer.Clone() as int[,];

        odd_button_holder.SetActive(false);
    }

    public void addVoxel(int x, int y) {
    	print("added " + x + " " + y);
    	setColor(buttons[x,y], Color.blue);
    	layer[x, y] = 1;
    	EventSystem.current.SetSelectedGameObject(null);
    	//print("added " + x + " " + y);
    }


    public void readVoxelFile(string path) {
        //read a line, save it to layer, update cur_layer
        //call addLayer()
        //repeat
            //cur_layer = 0;
            // Create an instance of StreamReader to read from a file.
            // The using statement also closes the StreamReader.
            StreamReader sr = new StreamReader(path);
            
            string line;
                // Read and display lines from the file until the end of
                // the file is reached.
            while ((line = sr.ReadLine()) != null)
            {
                print("line");
                   //print(line);
               string[] l = line.Split(',');
               int n = 0;
               foreach(string s in l) {
                    int x = n/19;
                    int y = n%19;
                    //print(s + " " + x + " " + y);
                    layer[x,y] = int.Parse(s);
                    n++;
               }
               addLayer();
               //cur_layer = cur_layer+1;
            
            }
    }

    public void addLayer() {
        int even_odd = cur_layer % 2;

        int voxels_added = 0;

    	for(int x = 0; x < num_buttons - even_odd; x++) {
        	for(int y = 0; y < num_buttons - even_odd; y++) {

        		if (layer[x*2 + even_odd,y*2 + even_odd] == 1) {
                    GameObject g;
                    if (even_odd % 2 == 0) {
	        		     g = Instantiate(voxel_prefab_even, new Vector3(0, 0, 0), Quaternion.identity);
                    } else {
                        g = Instantiate(voxel_prefab_odd, new Vector3(0, 0, 0), Quaternion.identity);
                    }
	        		g.transform.parent = voxel_holder.transform;
	        		g.transform.localPosition = new Vector3(x + 0.5f*even_odd, cur_layer*0.5f + 0.5f, y + 0.5f*even_odd);
	        		//g.transform.localScale = new Vector3(1,1,1);
	        		//g.GetComponent<RectTransform>().localEulerAngles = new Vector3(0,0,0);
	        		voxels_added++;
	        	} else {
                    layer[x*2 + even_odd,y*2 + even_odd] = 0;
                }
	        	setColor(buttons[x*2 + even_odd,y*2 + even_odd], Color.white);
	        	//layer[x*2 + even_odd,y*2 + even_odd] = 0;
        	}
        }
        if (voxels_added > 0) {
	        all_layers.Add(layer);
	        layer = zero_layer.Clone() as int[,];
	        cur_layer++;
	        if (even_odd == 1) {
	            odd_button_holder.SetActive(false);
	            even_button_holder.SetActive(true);
	        } else {
	            even_button_holder.SetActive(false);
	            odd_button_holder.SetActive(true);
	        }
	    }
    }

    public void exportModel() {
    	StreamWriter writer = new StreamWriter("voxel_structure.csv");
 		int n = 0;
        int n2 = 0;
 		foreach(int[,] l in all_layers) {
 			string state = "";
 			for (int x = 0; x < num_buttons*2 -1; x++) {
 				for (int y = 0; y < num_buttons*2 -1; y++) {
 					//state = state + l[x,y] + ",";
                    if (x == 0 && y == 0) {
                        writer.Write(l[x,y]);
                    } else {
                        writer.Write("," + l[x,y]);
                    }
                    if (l[x,y] == 0) {
                        n++;
                    }
                    if (l[x,y] == 1) {
                        n2++;
                    }
 				}
 			}
            writer.Write("\n");
            print(n);
            print(n2);
            //print(state.Length);
 			//state = state.Substring(0,state.Length - 1);
 			//writer.WriteLine(state);
 		}
        // /writer.WriteLine("");
        writer.Close();
        //writer.WriteLine("Inventory,OnlyX");

    }

    private void setColor(Button b, Color c) {
    	ColorBlock colors = b.colors;
    	colors.normalColor = c;
    	b.colors = colors;
    }

}
