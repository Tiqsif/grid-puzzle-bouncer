using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Vector2Int cellPosition;
    public Color color;
    public Type type;
    public float moveSpeed = 0f;
    private float currentSpeed;
    public Queue<Vector3> moveQueue = new Queue<Vector3>();
    public bool isJumpable = true;
    public bool isMoving = false;
    public bool isDead = false;
    public bool isGettingJumpedOn = false;
    [Tooltip("If true a temporary scale change will happen onJumped")]
    public bool isElastic = false;

    public AudioPackSO[] onJumpedAudioPacks;
    public GameObject JumpParticle;
    public GameObject onJumpedParticle;
    private Material onJumpedParticleMaterial;
    protected Platform platform;

    public delegate void OnUnitMove(Unit mover, Vector2Int direction, Unit unitMovedTo);
    public static event OnUnitMove onUnitMove;

    
    protected void Start()
    {
        platform = FindObjectOfType<Platform>();
        if (onJumpedParticle && onJumpedParticle.TryGetComponent(out ParticleSystemRenderer psRenderer))
        {
            /*
            Color totalColor = Color.black;
            int colorCount = 0;

             // to get average color of all the child meshes materials
            foreach (MeshRenderer renderer in GetComponentsInChildren<MeshRenderer>())
            {
                totalColor += renderer.material.color;
                colorCount++;
            }

            Color finalColor = colorCount > 0 ? totalColor / colorCount : Color.white;
            Debug.Log("Final color: " + finalColor);
            */
            Color finalColor = this.color;
            Material newMat = new Material(psRenderer.sharedMaterial);
            newMat.color = finalColor;
            onJumpedParticleMaterial = newMat;
        }
    }
    
    public virtual void Move(Vector2Int targetPosition)
    {
        Move(targetPosition, 0);
    }
    public virtual void Move(Vector2Int targetPosition, float delay)
    {
        if (isDead || moveSpeed <= 0 || isGettingJumpedOn)
        {
            return;
        }
        if (JumpParticle != null)
        {
            Instantiate(JumpParticle, transform.position, Quaternion.identity);
        }
        StartCoroutine(MoveRoutine(targetPosition, delay));
    }
    public virtual IEnumerator MoveRoutine(Vector2Int targetPosition, float delay)
    {
        while (isMoving)
        {
            yield return null;
        }
        yield return new WaitForSeconds(delay);
        //Debug.Log("Moving " + name);
        Vector2Int direction = targetPosition - cellPosition;
        Vector2 directionF = targetPosition - cellPosition;
        float mag = directionF.magnitude;
        currentSpeed = moveSpeed * mag;
        if (this.type == Type.Player)
        {
            transform.right = new Vector3(direction.x, 0, direction.y);
        }

        Unit targetUnit = GetNonSelfUnitAtPosition(targetPosition);
        Unit fromUnit = GetNonSelfUnitAtPosition(cellPosition);
        onUnitMove?.Invoke(this, direction, targetUnit);
        if (fromUnit != null) // non self unit jumped from
        {
            //JumpOffAnimation();
            fromUnit.JumpedOff(this);
            platform.SetGridElements();
        }
        if (targetUnit != null) // if a unit is in the target position
        {
            // targetUnit handles this units jumping logic
            yield return targetUnit.JumpingOn(this); // wait for the targets handling
            /*
            if (targetUnit.isJumpable)
            {
                JumpOnAnimation();
                yield return StartCoroutine(MoveTo(platform.grid.GetWorldPosition(targetPosition)));
            }
            else
            {
                BumpAnimation();
            }
            */
            platform.SetGridElements();
            targetUnit.JumpedOn(this); // after the jumping is done, call jumpedOn on the target, usually this is where jumper.cellPosition is set to target.cellPos
        }
        else // if no unit is in the target position
        {
            cellPosition = targetPosition; // instantyly snap to the target cell position without waiting the movement
            if (platform.IsInsideGrid(cellPosition)) // if in the grid
            {
                JumpAnimation();
                yield return StartCoroutine(MoveTo(platform.grid.GetWorldPosition(cellPosition))); // actual tweenlike movement
                Land();

            }
            else // if out of the grid
            {
                FallAnimation();
                yield return StartCoroutine(FallTo(platform.grid.GetWorldPosition(cellPosition))); // actual tweenlike movement, Y is lowered compared to MoveTo
            }
        }


        platform.SetGridElements();
    }
    public IEnumerator MoveTo(Vector3 targetPosition)
    {
        if (isDead)
        {
            yield break;
        }
        while (isMoving)
        {
            yield return null;
        }
        Vector3 target = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
        moveQueue.Enqueue(target);
        if (!isMoving)
        {
            yield return StartCoroutine(ProcessMoveQueue());
        }
    }

    private IEnumerator ProcessMoveQueue()
    {
        if (isDead)
        {
            yield break;
        }
        while (isMoving)
        {
            yield return null;
        }
        isMoving = true;
        

        while (moveQueue.Count > 0)
        {
            Vector3 targetPosition = moveQueue.Dequeue();
            Debug.Log(type + transform.position.ToString() + " started moving to " + targetPosition.ToString());
            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);
                yield return null;
            }
        }
        isMoving = false;
        
    }

    protected Unit GetUnitAtPosition(Vector2Int position)
    {
        
        if (platform == null || platform.units == null)
        {
            Debug.Log(platform.units == null);
            Debug.LogWarning("Platform not found");
            return null;
        }
        
        foreach (Unit unit in platform.units)
        {
            if (unit.cellPosition == position)
            {
                return unit;
            }
        }
        return null;
    }
    protected Unit GetUnitAtPosition(Vector2Int position, Unit excluded)
    {

        if (platform == null || platform.units == null)
        {
            Debug.Log(platform.units == null);
            Debug.LogWarning("Platform not found");
            return null;
        }

        foreach (Unit unit in platform.units)
        {
            if (unit.cellPosition == position && unit != excluded)
            {
                return unit;
            }
        }
        return null;
    }

    protected Unit GetNonSelfUnitAtPosition(Vector2Int position)
    {
        if (platform == null)
        {
            Debug.LogWarning("Platform not found");
            platform = FindObjectOfType<Platform>();
        }
        if (platform.units == null)
        {
            Debug.LogWarning("Platform units not found");
            return null;
        }
        foreach (Unit unit in platform.units)
        {
            if (unit.cellPosition == position && unit != this)
            {
                return unit;
            }
        }
        return null;
    }

    public virtual IEnumerator JumpingOn(Unit player)
    {
        isGettingJumpedOn = true;
        Debug.Log(player.type + " Jumping on " + type);
        yield return null;
    }
    public virtual void JumpedOn(Unit player)
    {
        isGettingJumpedOn = false;
        Debug.Log(player.type + " Jumped on " + type);

        if (onJumpedParticle != null)
        {
            GameObject particleObj = Instantiate(onJumpedParticle, transform.position, Quaternion.identity);
            particleObj.GetComponent<ParticleSystemRenderer>().material = onJumpedParticleMaterial;
            
        }
        foreach (AudioClip clip in AudioPackManager.GetRandomClipFromEachPack(onJumpedAudioPacks))
        {
            AudioManager.Instance.KillSFX(clip);
            AudioManager.Instance.PlaySFX(clip);
        }

        if (isElastic)
        {
            StartCoroutine(ApplyJiggle(0.5f, 0.1f));
        }
    }

    public virtual void JumpedOff(Unit player)
    {
        Debug.Log(player.type + " Jumped off of " + type);
    }

    public IEnumerator FallTo(Vector3 targetPosition)
    {
        // called when the player falls down to the water out of bounds
        if (isDead)
        {
            yield break;
        }
        
        moveQueue.Enqueue(targetPosition);
        yield return StartCoroutine(ProcessMoveQueue());
        targetPosition.y = transform.position.y - 4f;
        moveQueue.Enqueue(targetPosition);
        Debug.Log("Falling to " + targetPosition);
        yield return StartCoroutine(ProcessMoveQueue());
        if (TryGetComponent(out Player playerScript))
        {
            playerScript.Die(); // sets isdead to true in itself
        }
        else
        {
            isDead = true;
        }
        cellPosition = new Vector2Int(-10, -10); // graveyard position for dead units
        platform.SetGridElements();
        //DestroyImmediate(gameObject);
    }

    public virtual void JumpOnAnimation()
    {
        // animation for jumping on a unit
    }
    public virtual void JumpOffAnimation()
    {
        // animation for jumping off a unit
    }
    public virtual void JumpAnimation()
    {
        // animation for jumping forward
    }
    
    public virtual void BumpAnimation()
    {
        // animation for bumping into a wall
    }

    public virtual void FallAnimation()
    {
        // animation for falling down
    }

    public virtual void Land()
    {
        // called when landed on empty cell
        // can play sound effect for landing
    }

    protected IEnumerator ApplyJiggle(float duration, float intensity)
    {
        float timeElapsed = 0f;
        Vector3 initialScale = transform.localScale;
        while (timeElapsed < duration)
        {

            timeElapsed += Time.deltaTime;
            float jiggleAmountX = Mathf.Sin(timeElapsed * Mathf.PI * 4) * intensity; // Subtle jiggle on x-axis
            float jiggleAmountY = Mathf.Sin(timeElapsed * Mathf.PI * 4) * intensity * 1.5f; // Stronger jiggle on y-axis
            float jiggleAmountZ = Mathf.Sin(timeElapsed * Mathf.PI * 4) * intensity; // Subtle jiggle on z-axis

            transform.localScale = new Vector3(
                initialScale.x + jiggleAmountX,
                initialScale.y + jiggleAmountY,
                initialScale.z + jiggleAmountZ
            );

            yield return null;
        }
        transform.localScale = initialScale; // Reset scale
    }
}
