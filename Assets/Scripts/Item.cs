using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    // Base Item class - contains basic spawn-despawn logic

    // Assignable name and active time
    [SerializeField] public string itemName;
    [SerializeField] protected float vanishTime = 5.0f;
    // Runtime vars 
    protected ItemController itemController;
    protected MasterController masterController;
    protected Vector3 initialPosition;
    protected BoxCollider2D itemCollider;
    protected PlayerScript player;
    
    protected virtual void Awake()
    {
        itemCollider = GetComponent<BoxCollider2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
        itemController = GameObject.FindGameObjectWithTag("ItemController").GetComponent<ItemController>();
        masterController = GameObject.FindGameObjectWithTag("MasterController").GetComponent<MasterController>();
    }

    protected abstract void OnTriggerEnter2D(Collider2D otherCollider);

    protected virtual void Vanish()
    {
        gameObject.SetActive(false);
    }

    public virtual void SetInitialPosition()
    {
        initialPosition = transform.position;
        StartCoroutine(VanishCoroutine());
    }

    public virtual IEnumerator VanishCoroutine()
    {
        yield return new WaitForSeconds(vanishTime);

        if(transform.position == initialPosition) {
            Vanish();
        }
    }

}
