﻿using System;
using TinYard.Extensions.MediatorMap.API.Interfaces;
using TinYard.Extensions.ViewController.API.Interfaces;

namespace TinYard.Extensions.MediatorMap.API.VO
{
    public interface IMediatorMappingObject
    {
        IView View { get; }
        Type ViewType { get; }
        IMediator Mediator { get; }
        Type MediatorType { get; }

        event Action<IMediatorMappingObject> OnMediatorMapped;

        IMediatorMappingObject Map<T>();
        IMediatorMappingObject Map(IView view);
        IMediatorMappingObject Map(object view);

        IMediatorMappingObject ToMediator<T>() where T : IMediator;
        IMediatorMappingObject ToMediator(IMediator mediator);
    }
}
