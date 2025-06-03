﻿using System.Collections.Generic;
using System.Numerics;
using SoulsFormats;

namespace SoulsFormats
{
    /// <summary>
    /// A generic map layout file.
    /// </summary>
    public interface IMsb : ISoulsFile
    {
        /// <summary>
        /// Models available for use in the map.
        /// </summary>
        IMsbParam<IMsbModel> Models { get; }

        /// <summary>
        /// Events that control miscellaneous behaviors of the map.
        /// </summary>
        IMsbParam<IMsbEvent> Events { get; }

        /// <summary>
        /// Points and trigger volumes in the map.
        /// </summary>
        IMsbParam<IMsbRegion> Regions { get; }

        /// <summary>
        /// Concrete entities in the map.
        /// </summary>
        IMsbParam<IMsbPart> Parts { get; }
    }

    /// <summary>
    /// A generic map layout file with a number of tree bounding volume hierarchies.
    /// </summary>
    public interface IMsbBound<TTree> : IMsb where TTree : IMsbTree
    {
        /// <summary>
        /// Tree bounding volume hierarchies used in various calculations.
        /// </summary>
        IReadOnlyList<IMsbTreeParam<TTree>> Trees { get; }
    }

    /// <summary>
    /// A generic container of MSB items.
    /// </summary>
    public interface IMsbParam<T> where T : IMsbEntry
    {
        /// <summary>
        /// Adds the given item to the appropriate part of the param.
        /// </summary>
        T Add(T item);

        /// <summary>
        /// Returns all of the items in the param.
        /// </summary>
        IReadOnlyList<T> GetEntries();
    }

    /// <summary>
    /// A MapStudioTree containing a tree bounding volume hierarchy.
    /// </summary>
    public interface IMsbTreeParam<TTree> where TTree : IMsbTree
    {
        /// <summary>
        /// The axis-aligned tree bounding volume hierarchy.<br/>
        /// Set to null when not calculated yet.
        /// </summary>
        TTree Tree { get; set; }
    }

    /// <summary>
    /// A generic item in an MSB param.
    /// </summary>
    public interface IMsbEntry
    {
        /// <summary>
        /// The name of this entry.
        /// </summary>
        string Name { get; set; }
    }

    /// <summary>
    /// A model available to be used by parts.
    /// </summary>
    public interface IMsbModel : IMsbEntry
    {
        /// <summary>
        /// Creates a deep copy of the model.
        /// </summary>
        IMsbModel DeepCopy();
    }

    /// <summary>
    /// Controls miscellaneous behaviors.
    /// </summary>
    public interface IMsbEvent : IMsbEntry
    {
        /// <summary>
        /// Creates a deep copy of the event.
        /// </summary>
        IMsbEvent DeepCopy();
    }

    /// <summary>
    /// A point or trigger volume.
    /// </summary>
    public interface IMsbRegion : IMsbEntry
    {
        /// <summary>
        /// Describes the space that the region occupies.
        /// </summary>
        MSB.Shape Shape { get; set; }

        /// <summary>
        /// The center or bottom center of the region, depending on its shape.
        /// </summary>
        Vector3 Position { get; set; }

        /// <summary>
        /// The rotation of the region, in degrees.
        /// </summary>
        Vector3 Rotation { get; set; }

        /// <summary>
        /// Creates a deep copy of the region.
        /// </summary>
        IMsbRegion DeepCopy();
    }

    /// <summary>
    /// A visible and/or physical entity.
    /// </summary>
    public interface IMsbPart : IMsbEntry
    {
        /// <summary>
        /// The model used by this part.
        /// </summary>
        string ModelName { get; set; }

        /// <summary>
        /// The position of the part.
        /// </summary>
        Vector3 Position { get; set; }

        /// <summary>
        /// The rotation of the part, in degrees.
        /// </summary>
        Vector3 Rotation { get; set; }

        /// <summary>
        /// The scale of the part.
        /// </summary>
        Vector3 Scale { get; set; }

        /// <summary>
        /// Creates a deep copy of the part.
        /// </summary>
        IMsbPart DeepCopy();
    }

    /// <summary>
    /// A tree hierarchy of axis-aligned bounding boxes used in various calculations such as drawing, culling, and collision detection.
    /// </summary>
    public interface IMsbTree
    {
        /// <summary>
        /// The bounding box for this node.
        /// </summary>
        MsbBoundingBox Bounds { get; set; }

        /// <summary>
        /// The left child of this node.
        /// </summary>
        IMsbTree Left { get; set; }

        /// <summary>
        /// The right child of this node.
        /// </summary>
        IMsbTree Right { get; set; }

        /// <summary>
        /// Indices to the parts this node contains.
        /// </summary>
        List<short> PartIndices { get; set; }
    }
}
