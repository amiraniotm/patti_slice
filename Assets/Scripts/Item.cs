using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    // Base Item class - contains basic spawn-despawn logic

    // Assignable name and active time
    [SerializeField] public string itemName;
    [SerializeField] protected float vanishTime = 5.0f;
    // Runtime vars - some are only used in specific item types
    protected ItemController itemController;
    protected MasterController masterController;
    protected Vector3 initialPosition;
    protected BoxCollider2D itemCollider;
    protected PlayerScript player;
    protected bool wasTaken;
    
    protected virtual void Awake()
    {
        itemCollider = GetComponent<BoxCollider2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
        itemController = GameObject.FindGameObjectWithTag("ItemController").GetComponent<ItemController>();
        masterController = GameObject.FindGameObjectWithTag("MasterController").GetComponent<MasterController>();
    }
    // Every item must have a collision interaction
    protected abstract void OnTriggerEnter2D(Collider2D otherCollider);
    // Disabling item at end of life cycle, and returning it to untouched state
    public virtual void Vanish()
    {
        gameObject.SetActive(false);
        wasTaken = false;
    }

    // public virtual void SetInitialPosition()
    // {
    //     initialPosition = transform.position;
    //     StartCoroutine(VanishCoroutine());
    // }
    // Automatically disabling item if player didnt get it
    public virtual IEnumerator VanishCoroutine()
    {
        yield return new WaitForSeconds(vanishTime);

        if(!wasTaken) {
            Vanish();
        }
    }

}
