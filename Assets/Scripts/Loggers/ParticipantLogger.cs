﻿using NormandErwan.MasterThesis.Experiment.DeviceControllers;
using NormandErwan.MasterThesis.Experiment.Experiment.Task;
using NormandErwan.MasterThesis.Experiment.Experiment.Variables;
using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Loggers
{
  public class ParticipantLogger : Logger
  {
    public enum VariableType
    {
      Selections,
      Pan,
      Zoom
    }

    public class Variable
    {
      public VariableType type;
      public int count;
      public Stopwatch time = new Stopwatch();
      public float distance;

      public Variable(VariableType type)
      {
        this.type = type;
        Reset();
      }

      public void Reset()
      {
        count = 0;
        time.Reset();
        distance = 0;
      }
    }

    // Editor fields

    [SerializeField]
    private DeviceController deviceController;

    // Properties

    public int HeadPhoneDistance { get; set; }

    // Variables

    protected DateTime startDateTime;

    public Variable selections = new Variable(VariableType.Selections);
    protected int deselections = 0;
    protected int errors = 0;
    protected int success = 0;

    public Variable pan = new Variable(VariableType.Pan);
    public Variable zoom = new Variable(VariableType.Zoom);

    protected Experiment.Task.Grid grid;

    // MonoBehaviour methods

    protected override void Awake()
    {
      base.Awake();

      dataPath += "/Logs";

      grid = deviceController.Grid;

      deviceController.Configured += DeviceController_Configured;

      grid.Configured += Grid_Configured;
      grid.Completed += Grid_Completed;
      grid.ItemSelected += Grid_ItemSelected;
      grid.ItemClassed += Grid_ItemClassed;

      grid.DraggingStarted += Grid_DraggingStarted;
      grid.Dragging += Grid_Dragging;
      grid.DraggingStopped += Grid_DraggingStopped;

      grid.ZoomingStarted += Grid_ZoomingStarted;
      grid.Zooming += Grid_Zooming;
      grid.ZoomingStopped += Grid_ZoomingStopped;
    }

    protected override void OnDestroy()
    {
      base.OnDestroy();

      deviceController.Configured -= DeviceController_Configured;

      grid.Configured -= Grid_Configured;
      grid.Completed -= Grid_Completed;
      grid.ItemSelected -= Grid_ItemSelected;
      grid.ItemClassed -= Grid_ItemClassed;

      grid.DraggingStarted -= Grid_DraggingStarted;
      grid.Dragging -= Grid_Dragging;
      grid.DraggingStopped -= Grid_DraggingStopped;

      grid.ZoomingStarted -= Grid_ZoomingStarted;
      grid.Zooming -= Grid_Zooming;
      grid.ZoomingStopped -= Grid_ZoomingStopped;
    }

    // Methods

    public override void StartLogger()
    {
      Filename = "participant-" + deviceController.ParticipantId + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".csv";
      Columns = new List<string>() {
        "ParticipantId", "Technique", "Distance", "TextSize", "TrialNumber",
        "StartDateTime", "TotalTime",
        "SelectionsCount", "SelectionsTime", "SelectionsDistance", "Deselections", "Errors", "Success",
        "PanCount", "PanTime", "PanDistance",
        "ZoomCount", "ZoomTime", "ZoomDistance",
        "HeadPhoneDistance"
      };
      base.StartLogger();
    }

    protected virtual void DeviceController_Configured()
    {
      StartLogger();
    }

    protected virtual void Grid_Configured()
    {
      PrepareNextRow();

      startDateTime = DateTime.Now;

      selections.Reset();
      deselections = 0;
      errors = 0;

      pan.Reset();
      zoom.Reset();

      AddToNextRow(deviceController.ParticipantId);
      AddToNextRow(deviceController.StateController.GetIndependentVariable<IVTechnique>().CurrentCondition.id);
      AddToNextRow(deviceController.StateController.GetIndependentVariable<IVClassificationDifficulty>().CurrentCondition.id);
      AddToNextRow(deviceController.StateController.GetIndependentVariable<IVTextSize>().CurrentCondition.id);
      AddToNextRow(deviceController.StateController.CurrentTrial);
    }

    protected virtual void Grid_Completed()
    {
      AddToNextRow(startDateTime);
      AddToNextRow((DateTime.Now - startDateTime).TotalSeconds);

      AddToNextRow(selections);
      AddToNextRow(deselections);
      AddToNextRow(errors);
      AddToNextRow(success);

      AddToNextRow(pan);
      AddToNextRow(zoom);

      AddToNextRow(HeadPhoneDistance);

      WriteRow();
    }

    protected virtual void Grid_ItemSelected(Container container, Item item, bool selected)
    {
      if (selected)
      {
        selections.count++;
        selections.time.Start();
      }
      else
      {
        deselections++;
        selections.time.Stop();
      }
    }

    protected virtual void Grid_ItemClassed(Container oldContainer, Container newContainer, Item item, bool success)
    {
      if (success)
      {
        this.success++;
      }
      else
      {
        errors++;
      }
      selections.time.Stop();
    }

    protected virtual void Grid_DraggingStarted(IDraggable grid)
    {
      pan.count++;
      pan.time.Start();
    }

    protected virtual void Grid_Dragging(IDraggable grid, Vector3 translation)
    {
      var magnitude = translation.magnitude;
      pan.distance += magnitude;

      if (selections.time.IsRunning)
      {
        selections.distance += magnitude;
      }
    }

    protected virtual void Grid_DraggingStopped(IDraggable grid)
    {
      pan.time.Stop();
    }

    protected virtual void Grid_ZoomingStarted(IZoomable grid)
    {
      zoom.count++;
      zoom.time.Start();
    }

    protected virtual void Grid_Zooming(IZoomable grid, float scaleFactor, Vector3 translation, Vector3[] cursors)
    {
      var distance = cursors[0] - cursors[1];
      var previousDistance = cursors[2] - cursors[3];
      var magnitude = (distance - previousDistance).magnitude;

      zoom.distance += magnitude;

      if (selections.time.IsRunning)
      {
        selections.distance += magnitude;
      }
    }

    protected virtual void Grid_ZoomingStopped(IZoomable grid)
    {
      zoom.time.Stop();
    }

    protected virtual void AddToNextRow(Variable variable)
    {
      AddToNextRow(variable.count);
      AddToNextRow(variable.time.Elapsed.TotalSeconds);
      AddToNextRow(variable.distance);
    }
  }
}