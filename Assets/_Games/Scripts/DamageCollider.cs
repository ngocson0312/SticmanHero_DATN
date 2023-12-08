using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditorInternal;

    [CustomEditor(typeof(DamageCollider))]
    public class DamageColliderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DamageCollider damageCollider = target as DamageCollider;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("shape"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("contactLayer"));
            switch (damageCollider.shape)
            {
                case DamageCollider.ColliderShape.BOX:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("size"));
                    break;
                case DamageCollider.ColliderShape.CIRCLE:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("radius"));
                    break;
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif

    public class DamageCollider : MonoBehaviour
    {
        public LayerMask contactLayer;
        public Vector2 size = Vector2.one;
        public float radius = 1f;
        public float damageTimeRate = 1f;
        public ColliderShape shape;
        private Collider2D[] colls = new Collider2D[5];
        private List<Collider2D> listContact;
        private Controller controller;
        private DamageInfo damageInfo;
        private bool isActive;
        public void Initialize(Controller controller)
        {
            this.controller = controller;
            listContact = new List<Collider2D>();
        }
        public void SetActive(bool status)
        {
            isActive = status;
            gameObject.SetActive(status);
        }
        public void ResetCollider()
        {
            listContact.Clear();
        }
        public void SetDamageData(DamageInfo damageInfo)
        {
            this.damageInfo = damageInfo;
        }
        private void Update()
        {
            if (!isActive) return;
            for (int i = 0; i < colls.Length; i++)
            {
                colls[i] = null;
            }
            switch (shape)
            {
                case ColliderShape.BOX:
                    Physics2D.OverlapBoxNonAlloc(transform.position, size, 0, colls, contactLayer);
                    break;
                case ColliderShape.CIRCLE:
                    Physics2D.OverlapCircleNonAlloc(transform.position, radius, colls, contactLayer);
                    break;
            }
            for (int i = 0; i < colls.Length; i++)
            {
                if (colls[i] == null) continue;
                if (listContact.Contains(colls[i])) continue;
                if (controller.core.combat.IsSelfCollider(colls[i])) continue;
                IDamage id = colls[i].GetComponent<IDamage>();
                if (id == null) continue;
                id.TakeDamage(damageInfo);
                listContact.Add(colls[i]);
            }
        }
        public Collider2D[] CheckCollider()
        {
            for (int i = 0; i < colls.Length; i++)
            {
                colls[i] = null;
            }
            switch (shape)
            {
                case ColliderShape.BOX:
                    Physics2D.OverlapBoxNonAlloc(transform.position, size, 0, colls, contactLayer);
                    break;
                case ColliderShape.CIRCLE:
                    Physics2D.OverlapCircleNonAlloc(transform.position, radius, colls, contactLayer);
                    break;
            }
            return colls;
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            switch (shape)
            {
                case ColliderShape.BOX:
                    Gizmos.DrawWireCube(transform.position, size);
                    break;
                case ColliderShape.CIRCLE:
                    Gizmos.DrawWireSphere(transform.position, radius);
                    break;
            }
        }
        public enum ColliderShape
        {
            BOX, CIRCLE
        }
        public enum DamageType
        {
            RESET, BY_THE_TIME
        }
    }
}
