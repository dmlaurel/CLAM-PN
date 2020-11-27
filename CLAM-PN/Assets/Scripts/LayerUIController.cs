using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

	private int[,] layer;
	private Button[,] buttons;
	private int cur_layer;

    // Start is called before the first frame update
    void Start()
    {

    	cur_layer = 0;
    	layer = new int[num_buttons*2 - 1, num_buttons*2 - 1];
    	buttons = new Button[num_buttons*2 - 1, num_buttons*2 - 1];

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
        		layer[x*2, y*2] = 0;
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
                layer[x*2 + 1, y*2 + 1] = 0;
            }
        }

        odd_button_holder.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void addVoxel(int x, int y) {
    	print("added " + x + " " + y);
    	setColor(buttons[x,y], Color.blue);
    	layer[x, y] = 1;
    	EventSystem.current.SetSelectedGameObject(null);
    	//print("added " + x + " " + y);
    }

    public void addLayer() {
        int even_odd = 0;
        if (cur_layer % 2 == 1) {
            even_odd = 1;
            odd_button_holder.SetActive(false);
            even_button_holder.SetActive(true);
        } else {
            even_button_holder.SetActive(false);
            odd_button_holder.SetActive(true);
        }

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
	        	}
	        	setColor(buttons[x*2 + even_odd,y*2 + even_odd], Color.white);
	        	layer[x*2 + even_odd,y*2 + even_odd] = 0;
        	}
        }
        cur_layer++;
    }

    private void setColor(Button b, Color c) {
    	ColorBlock colors = b.colors;
    	colors.normalColor = c;
    	b.colors = colors;
    }

}
