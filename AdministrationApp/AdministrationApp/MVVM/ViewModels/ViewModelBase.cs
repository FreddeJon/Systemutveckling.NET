﻿using System;
using System.Threading.Tasks;

namespace AdministrationApp.MVVM.ViewModels;

public class ViewModelBase : ObservableObject, IDisposable
{
    public virtual Task LoadAsync()
    {
        return Task.CompletedTask;
    }

    public virtual void Dispose()
    {
        GC.Collect();
    }
}