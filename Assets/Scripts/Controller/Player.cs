using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerData data;
    protected Animator animator;
    protected int skinID = 1;
    // Start is called before the first frame update
    public virtual void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public virtual void Start()
    {
        LeaderBoardManager.Instance.RegisterScoreData(data);
    }

    // Update is called once per frame
    public virtual void Update()
    {
        
    }
    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("ChangeSkin"))
        {
            skinID += 1;
            animator.Play("Birt" + skinID);
        }
        if(collision.gameObject.CompareTag("StartRole"))
        {
            animator.Play("Birt1");
            skinID = 1;
        }
    }
}
