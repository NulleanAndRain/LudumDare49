using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttakSystem : MonoBehaviour
{
    //public int hp = 100;
    public float saerchRadius;
    public Vector2 EyesPos;
    private Vector2 _eyePos { get => (Vector2)transform.position + EyesPos; }
    Vector2 _rc_dir = Vector2.zero;
    [Header("Damage")]
    public int attakDamage;
    public int bodyDamaje;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TouchDamage()
    {

    }

    public void MakeAnAttak()
    {

    }
}
