using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemActionSpawnEntity : ItemAction
{
    public override ItemActionData CreateModifierData(ItemInventoryData _invData, int _indexInEntityOfAction)
    {
        return new ItemActionSpawnEntity.ItemActionDataSpawnEntity(_invData, _indexInEntityOfAction);
    }


    /**
     * Reads from dynamic properties.
     */

    public override void ReadFrom(DynamicProperties _props)
    {
        base.ReadFrom(_props);
        if (_props.Values.ContainsKey("Entity"))
        {
            this.entityToSpawn = _props.Values["Entity"];
        }
       
        foreach (KeyValuePair<int, EntityClass> keyValuePair in EntityClass.list.Dict)
        {
            if (keyValuePair.Value.entityClassName == this.entityToSpawn)
            {
                this.entityId = keyValuePair.Key;
                break;
            }
        }
    }


    /**
     * Run this code when the entity is held in hand.
     */

    public override void StartHolding(ItemActionData _actionData)
    {
        ItemActionSpawnEntity.ItemActionDataSpawnEntity itemActionDataSpawnEntity = (ItemActionSpawnEntity.ItemActionDataSpawnEntity)_actionData;
        EntityPlayerLocal entityPlayerLocal = itemActionDataSpawnEntity.invData.holdingEntity as EntityPlayerLocal;
        if (!entityPlayerLocal)
        {
            return;
        }
        if (itemActionDataSpawnEntity.EntityPreviewT != null)
        {
            UnityEngine.Object.DestroyImmediate(itemActionDataSpawnEntity.EntityPreviewT.gameObject);
        }
        GameObject original = DataLoader.LoadAsset<GameObject>(entityPlayerLocal.inventory.holdingItem.MeshFile);
        itemActionDataSpawnEntity.EntityPreviewT = UnityEngine.Object.Instantiate<GameObject>(original).transform;
        Vehicle.SetupPreview(itemActionDataSpawnEntity.EntityPreviewT);
        this.setupPreview(itemActionDataSpawnEntity);
        this.updatePreview(itemActionDataSpawnEntity);
    }

    
    /**
     * Sets up the preview for the entity to spawn.
     */

    private void setupPreview(ItemActionSpawnEntity.ItemActionDataSpawnEntity data)
    {
        if (data.PreviewRenderers == null || data.PreviewRenderers.Length == 0 || data.PreviewRenderers[0] == null)
        {
            data.PreviewRenderers = data.EntityPreviewT.GetComponentsInChildren<Renderer>();
        }
        for (int i = 0; i < data.PreviewRenderers.Length; i++)
        {
            data.PreviewRenderers[i].material.color = new Color(2f, 0.25f, 0.25f);
        }
    }


    /**
     * Updates the preview if the character moves around.
     */

    private void updatePreview(ItemActionSpawnEntity.ItemActionDataSpawnEntity data)
    {
        if (data.PreviewRenderers == null || data.PreviewRenderers.Length == 0 || data.PreviewRenderers[0] == null)
        {
            data.PreviewRenderers = data.EntityPreviewT.GetComponentsInChildren<Renderer>();
        }
        World world = data.invData.world;
        bool flag = this.CalcSpawnPosition(data, ref data.Position) && world.CanPlaceBlockAt(new Vector3i(data.Position), world.GetGameManager().GetPersistentLocalPlayer(), false);
        if (data.ValidPosition != flag)
        {
            data.ValidPosition = flag;
            for (int i = 0; i < data.PreviewRenderers.Length; i++)
            {
                data.PreviewRenderers[i].material.color = (flag ? new Color(0.25f, 2f, 0.25f) : new Color(2f, 0.25f, 0.25f));
            }
        }
        Quaternion localRotation = data.EntityPreviewT.localRotation;
        localRotation.eulerAngles = new Vector3(0f, data.invData.holdingEntity.rotation.y + 90f, 0f);
        data.EntityPreviewT.localRotation = localRotation;
        data.EntityPreviewT.position = data.Position - Origin.position;
    }

    
    /**
     * Runs when action is cancelled.
     */

    public override void CancelAction(ItemActionData _actionData)
    {
        ItemActionSpawnEntity.ItemActionDataSpawnEntity itemActionDataSpawnEntity = (ItemActionSpawnEntity.ItemActionDataSpawnEntity)_actionData;
        if (itemActionDataSpawnEntity.EntityPreviewT != null && itemActionDataSpawnEntity.invData.holdingEntity is EntityPlayerLocal)
        {
            UnityEngine.Object.Destroy(itemActionDataSpawnEntity.EntityPreviewT.gameObject);
        }
    }


    /**
     * When the item is no longer held in hand, destroy the preview object.
     */

    public override void StopHolding(ItemActionData _actionData)
    {
        ItemActionSpawnEntity.ItemActionDataSpawnEntity itemActionDataSpawnEntity = (ItemActionSpawnEntity.ItemActionDataSpawnEntity)_actionData;
        if (itemActionDataSpawnEntity.EntityPreviewT != null && itemActionDataSpawnEntity.invData.holdingEntity is EntityPlayerLocal)
        {
            UnityEngine.Object.Destroy(itemActionDataSpawnEntity.EntityPreviewT.gameObject);
        }
    }


    /**
     * When ticks update, update preview if the character has moved around.
     */

    public override void OnHoldingUpdate(ItemActionData _actionData)
    {
        ItemActionSpawnEntity.ItemActionDataSpawnEntity itemActionDataSpawnEntity = (ItemActionSpawnEntity.ItemActionDataSpawnEntity)_actionData;
        if (itemActionDataSpawnEntity.EntityPreviewT != null && itemActionDataSpawnEntity.invData.holdingEntity is EntityPlayerLocal)
        {
            this.updatePreview(itemActionDataSpawnEntity);
        }
    }


    /**
     * Executes the action data.
     */

    public override void ExecuteAction(ItemActionData _actionData, bool _bReleased)
    {
        ItemActionSpawnEntity.ItemActionDataSpawnEntity itemActionDataSpawnEntity = (ItemActionSpawnEntity.ItemActionDataSpawnEntity)_actionData;
        if (!(_actionData.invData.holdingEntity is EntityPlayerLocal))
        {
            return;
        }
        if (!_bReleased)
        {
            return;
        }
        if (Time.time - _actionData.lastUseTime < this.Delay)
        {
            return;
        }
        if (Time.time - _actionData.lastUseTime < Constants.cBuildIntervall)
        {
            return;
        }
        if (!itemActionDataSpawnEntity.ValidPosition)
        {
            return;
        }
        ItemInventoryData invData = _actionData.invData;
        if (this.entityId < 0)
        {
            foreach (KeyValuePair<int, EntityClass> keyValuePair in EntityClass.list.Dict)
            {
                if (keyValuePair.Value.entityClassName == this.entityToSpawn)
                {
                    this.entityId = keyValuePair.Key;
                    break;
                }
            }
            if (this.entityId == 0)
            {
                return;
            }
        }
        ItemValue holdingItemItemValue = invData.holdingEntity.inventory.holdingItemItemValue;
        if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
        {
            SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageVehicleSpawn>().Setup(this.entityId, itemActionDataSpawnEntity.Position, new Vector3(0f, invData.holdingEntity.rotation.y + 90f, 0f), holdingItemItemValue.Clone(), invData.holdingEntity.entityId), true);
        }
        else
        {
            Entity entity = EntityFactory.CreateEntity(this.entityId, itemActionDataSpawnEntity.Position + Vector3.up * 0.25f, new Vector3(0f, _actionData.invData.holdingEntity.rotation.y + 90f, 0f));
            entity.SetSpawnerSource(EnumSpawnerSource.StaticSpawner);
            if (entity as EntityAlive != null)
            {
                (entity as EntityAlive).factionId = itemActionDataSpawnEntity.invData.holdingEntity.factionId;
                (entity as EntityAlive).belongsPlayerId = itemActionDataSpawnEntity.invData.holdingEntity.entityId;
                (entity as EntityAlive).factionRank = (byte)(itemActionDataSpawnEntity.invData.holdingEntity.factionRank - 1);
            }
            GameManager.Instance.World.SpawnEntityInWorld(entity);
        }
        if (itemActionDataSpawnEntity.EntityPreviewT != null && itemActionDataSpawnEntity.invData.holdingEntity is EntityPlayerLocal)
        {
            UnityEngine.Object.Destroy(itemActionDataSpawnEntity.EntityPreviewT.gameObject);
        }
        invData.holdingEntity.RightArmAnimationUse = true;
        (invData.holdingEntity as EntityPlayerLocal).DropTimeDelay = 0.5f;
        invData.holdingEntity.inventory.DecHoldingItem(1);
        invData.holdingEntity.PlayOneShot((this.soundStart != null) ? this.soundStart : "placeblock", false);
    }

    
    /**
     * Calculates the spawn position when an entity is going to spawn.
     */

    private bool CalcSpawnPosition(ItemActionSpawnEntity.ItemActionDataSpawnEntity _actionData, ref Vector3 position)
    {
        World world = _actionData.invData.world;
        Ray lookRay = _actionData.invData.holdingEntity.GetLookRay();
        if (Vector3.Dot(lookRay.direction, Vector3.up) == 1f)
        {
            return false;
        }
        if (Voxel.Raycast(world, lookRay, 4f + this.entitySize.x, 8454144, 69, 0f))
        {
            for (float num = 0.14f; num < 0.91f; num += 0.25f)
            {
                position = Voxel.voxelRayHitInfo.hit.pos;
                position.y += num;
                Vector3 localPos = position - Origin.position;
                Vector3 normalized = Vector3.Cross(lookRay.direction, Vector3.up).normalized;
                Vector3 vector = Vector3.Cross(normalized, Vector3.up);
                localPos.y += this.entitySize.y * 0.5f + 0.05f;
                if (this.CheckForSpace(localPos, normalized, this.entitySize.z, vector, this.entitySize.x, Vector3.up, this.entitySize.y) && this.CheckForSpace(localPos, vector, this.entitySize.x, normalized, this.entitySize.z, Vector3.up, this.entitySize.y) && this.CheckForSpace(localPos, Vector3.up, this.entitySize.y, normalized, this.entitySize.z, vector, this.entitySize.x))
                {
                    return true;
                }
            }
        }
        return false;
    }

    
    /**
     * Checks there is enough space before a spawn can occur.
     */
    
    private bool CheckForSpace(Vector3 localPos, Vector3 dirN, float length, Vector3 axis1N, float axis1Length, Vector3 axis2N, float axis2Length)
    {
        Vector3 b = dirN * length * 0.5f;
        for (float num = -axis1Length * 0.5f; num <= axis1Length * 0.5f; num += 0.2499f)
        {
            Vector3 a = localPos + axis1N * num;
            for (float num2 = -axis2Length * 0.5f; num2 <= axis2Length * 0.5f; num2 += 0.2499f)
            {
                Vector3 a2 = a + axis2N * num2;
                if (Physics.Raycast(a2 - b, dirN, length, 28901376))
                {
                    return false;
                }
                if (Physics.Raycast(a2 + b, -dirN, length, 28901376))
                {
                    return false;
                }
            }
        }
        return true;
    }


    /**
     * Garbage collection.
     */

    public override void Cleanup(ItemActionData _data)
    {
        base.Cleanup(_data);
        ItemActionSpawnEntity.ItemActionDataSpawnEntity itemActionDataSpawnEntity = _data as ItemActionSpawnEntity.ItemActionDataSpawnEntity;
        if (itemActionDataSpawnEntity != null && itemActionDataSpawnEntity.EntityPreviewT != null && itemActionDataSpawnEntity.invData != null && itemActionDataSpawnEntity.invData.holdingEntity is EntityPlayerLocal)
        {
            UnityEngine.Object.Destroy(itemActionDataSpawnEntity.EntityPreviewT.gameObject);
        }
    }

    private const int cColliderMask = 28901376;
    private string entityToSpawn;
    private int entityId = -1;
    private Vector3 entitySize;


    /**
     * Action data for the general entity spawning.
     */

    protected class ItemActionDataSpawnEntity : ItemActionAttackData
    {
        /**
         * CTOR
         */

        public ItemActionDataSpawnEntity(ItemInventoryData _invData, int _indexInEntityOfAction) : base(_invData, _indexInEntityOfAction)
        {
        }

        public Transform EntityPreviewT;
        public Renderer[] PreviewRenderers;
        public bool ValidPosition;
        public Vector3 Position;
    }
}
