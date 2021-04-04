using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Core.Interface
{
    public interface IBaseSystemInterface
    {
    }

    public interface IAwakeInterface : IBaseSystemInterface
    {
        void Awake();
    }

    public interface IStartInterface : IBaseSystemInterface
    {
        void Start();
    }

    public interface IUpdateInterface : IBaseSystemInterface
    {
        void Update();
    }

    public interface ILateUpdateInterface : IBaseSystemInterface
    {
        void LateUpdate();
    }

}
