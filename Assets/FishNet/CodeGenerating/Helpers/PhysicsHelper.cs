﻿using System.Collections.Generic;
using FishNet.CodeGenerating.Extension;
using MonoFN.Cecil;
using MonoFN.Cecil.Cil;
using UnityEngine;
using SR = System.Reflection;


namespace FishNet.CodeGenerating.Helping
{
    internal class PhysicsHelper : CodegenBase
    {
        public override bool ImportReferences()
        {
            SR.MethodInfo locMi;
            //GetScene.
            locMi = typeof(GameObject).GetMethod("get_scene");
            GetScene_MethodRef = ImportReference(locMi);

            //Physics.SyncTransform.
            foreach (var mi in typeof(Physics).GetMethods())
                if (mi.Name == nameof(Physics.SyncTransforms))
                {
                    Physics3D_SyncTransforms_MethodRef = ImportReference(mi);
                    break;
                }

            foreach (var mi in typeof(Physics2D).GetMethods())
                if (mi.Name == nameof(Physics2D.SyncTransforms))
                {
                    Physics2D_SyncTransforms_MethodRef = ImportReference(mi);
                    break;
                }

            //PhysicsScene.Simulate.
            foreach (var mi in typeof(PhysicsScene).GetMethods())
                if (mi.Name == nameof(PhysicsScene.Simulate))
                {
                    Physics3D_Simulate_MethodRef = ImportReference(mi);
                    break;
                }

            foreach (var mi in typeof(PhysicsScene2D).GetMethods())
                if (mi.Name == nameof(PhysicsScene2D.Simulate))
                {
                    Physics2D_Simulate_MethodRef = ImportReference(mi);
                    break;
                }

            //GetPhysicsScene.
            foreach (var mi in typeof(PhysicsSceneExtensions).GetMethods())
                if (mi.Name == nameof(PhysicsSceneExtensions.GetPhysicsScene))
                {
                    GetPhysicsScene3D_MethodRef = ImportReference(mi);
                    break;
                }

            foreach (var mi in typeof(PhysicsSceneExtensions2D).GetMethods())
                if (mi.Name == nameof(PhysicsSceneExtensions2D.GetPhysicsScene2D))
                {
                    GetPhysicsScene2D_MethodRef = ImportReference(mi);
                    break;
                }

            return true;
        }


        /// <summary>
        ///     Returns instructions to get a physics scene from a gameObject.
        /// </summary>
        public List<Instruction> GetPhysicsScene(MethodDefinition md, VariableDefinition gameObjectVd,
            bool threeDimensional)
        {
            var processor = md.Body.GetILProcessor();
            return GetPhysicsScene(processor, gameObjectVd, threeDimensional);
        }

        /// <summary>
        ///     Returns instructions to get a physics scene from a gameObject.
        /// </summary>
        public List<Instruction> GetPhysicsScene(ILProcessor processor, VariableDefinition gameObjectVd,
            bool threeDimensional)
        {
            var insts = new List<Instruction>();

            //gameObject.scene.GetPhysics...
            insts.Add(processor.Create(OpCodes.Ldloc, gameObjectVd));
            insts.Add(processor.Create(GetScene_MethodRef.GetCallOpCode(Session), GetScene_MethodRef));
            if (threeDimensional)
                insts.Add(processor.Create(OpCodes.Call, GetPhysicsScene3D_MethodRef));
            else
                insts.Add(processor.Create(OpCodes.Call, GetPhysicsScene2D_MethodRef));

            return insts;
        }

        #region Reflection references.

        public MethodReference GetScene_MethodRef;
        public MethodReference GetPhysicsScene2D_MethodRef;
        public MethodReference GetPhysicsScene3D_MethodRef;
        public MethodReference Physics3D_Simulate_MethodRef;
        public MethodReference Physics2D_Simulate_MethodRef;
        public MethodReference Physics3D_SyncTransforms_MethodRef;
        public MethodReference Physics2D_SyncTransforms_MethodRef;

        #endregion
    }
}