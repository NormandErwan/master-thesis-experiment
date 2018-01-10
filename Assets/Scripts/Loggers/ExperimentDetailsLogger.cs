﻿using NormandErwan.MasterThesis.Experiment.Experiment.Task;
using NormandErwan.MasterThesis.Experiment.Inputs.Interactables;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NormandErwan.MasterThesis.Experiment.Loggers
{
  public class ExperimentDetailsLogger : ExperimentBaseLogger
  {
    // Editor fields

    [SerializeField]
    private Inputs.Cursor index;

    // Variables

    protected bool gridConfigured;
    protected bool gridCompleted;

    // MonoBehaviour methods

    protected virtual void LateUpdate()
    {
      if (IsConfigured)
      {
        PrepareRow();

        AddToRow(Time.frameCount);
        AddToRow(deviceController.ParticipantId);
        AddToRow(DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss.fff"));
        AddToRow(head, false);

        AddToRow(grid.transform, false);
        AddToRow(grid.LossyScale);
        AddToRow(gridConfigured);
        AddToRow(gridCompleted);

        AddToRow(index.transform, false);

        WriteRow();

        gridConfigured = gridCompleted = false;
      }
    }

    // Methods

    public override void Configure()
    {
      Filename = "participant-" + deviceController.ParticipantId + "-details_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".csv";

      Columns = new List<string>() { "FrameId", "ParticipantId", "DateTime" };
      AddTransformToColumns("head", false);

      AddTransformToColumns("grid");
      Columns.Add("gridConfigured");
      Columns.Add("gridCompleted");

      AddTransformToColumns("index", false);

      base.Configure();
    }

    protected override void Grid_Configured()
    {
      gridConfigured = true;
    }

    protected override void Grid_Completed()
    {
      gridCompleted = true;
    }

    protected override void Grid_ItemSelected(Container container, Item item, bool selected)
    {
    }

    protected override void Grid_ItemClassed(Container oldContainer, Container newContainer, Item item, bool success)
    {
    }

    protected override void Grid_DraggingStarted(IDraggable grid)
    {
    }

    protected override void Grid_Dragging(IDraggable grid, Vector3 translation)
    {
    }

    protected override void Grid_DraggingStopped(IDraggable grid)
    {
    }

    protected override void Grid_ZoomingStarted(IZoomable grid)
    {
    }

    protected override void Grid_Zooming(IZoomable grid, float scaleFactor, Vector3 translation, Vector3[] cursors)
    {
    }

    protected override void Grid_ZoomingStopped(IZoomable grid)
    {
    }
  }
}