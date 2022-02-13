using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NNI.Terrain {

  [Serializable]
  public class Node : MonoBehaviour {

    // public struct Priority : IComparable<Priority> {
    //   public int depth;
    //   public float distance;

    //   public Priority (int depth, float distance) {
    //     this.depth = depth;
    //     this.distance = distance;
    //   }

    //   int IComparable<Priority>.CompareTo (Priority other) {
    //     if (depth < other.depth) return -1;
    //     else if (depth > other.depth) return 1;
    //     else if (distance < other.distance) return -1;
    //     else if (distance > other.distance) return 1;
    //     return 0;
    //   }
    // }

    // public int depth;

    // public World world;

    // public NodeType nodeType;
    // public IndexBufferSelection activeIndexBuffer;

    // public Node up;

    // public Vector3 centrePoint;

    // public double edgeLength;

    // public bool hasChildren;

    // public Node northEast;
    // public Node southEast;
    // public Node southWest;
    // public Node northWest;

    // public Node north;
    // public Node east;
    // public Node south;
    // public Node west;

    // public Patch patch;

    // public Vector3 nw, ne, sw, se;

    // public float observerDistance;

    // public Node northSouthWest => north != null ? north.southWest : null;
    // public Node northSouthEast => north != null ? north.southEast : null;
    // public Node eastNorthWest => east != null ? east.northWest : null;
    // public Node eastSouthWest => east != null ? east.southWest : null;
    // public Node southNorthEast => south != null ? south.northEast : null;
    // public Node southNorthWest => south != null ? south.northWest : null;
    // public Node westNorthEast => west != null ? west.northEast : null;
    // public Node westSouthEast => west != null ? west.southEast : null;

    // internal static Node New () {
    //   return new GameObject().AddComponent<Node>();
    // }

    // public Node Setup (World world, string name, NodeType nodeType, Node up, Vector3 nw, Vector3 ne, Vector3 sw, Vector3 se) {
    //   this.gameObject.hideFlags = HideFlags.DontSaveInEditor;
    //   this.nw = nw;
    //   this.ne = ne;
    //   this.sw = sw;
    //   this.se = se;
    //   gameObject.layer = world.gameObject.layer;
    //   transform.SetParent(world.transform);
    //   if (up)
    //     depth = up.depth + 1;
    //   else
    //     depth = 0;
    //   this.name = name;
    //   this.world = world;
    //   this.up = up;
    //   this.nodeType = nodeType;
    //   this.centrePoint = (nw + ne + sw + se) / 4;
    //   this.edgeLength = Vector3.Distance(this.nw, this.se);
    //   this.activeIndexBuffer = IndexBufferSelection.Base;
    //   this.transform.localPosition = centrePoint;
    //   return this;
    // }

    // internal void ConfigureMaterial (MaterialPropertyBlock block) {
    //   world.ConfigureMaterial(block);
    //   block.SetVector("_Origin", centrePoint);
    //   block.SetFloat("_LodDepth", (float) depth / world.maxDepth);
    // }

    // internal void ConfigureCompute (ComputeShader compute) {
    //   world.ConfigureCompute(compute);
    //   compute.SetVector("_NW", nw / world.size);
    //   compute.SetVector("_NE", ne / world.size);
    //   compute.SetVector("_SW", sw / world.size);
    //   compute.SetVector("_SE", se / world.size);
    // }

    // public void DestroyChildren () {
    //   if (northEast)
    //     northEast.Destroy();
    //   if (southEast)
    //     southEast.Destroy();
    //   if (southWest)
    //     southWest.Destroy();
    //   if (northWest)
    //     northWest.Destroy();
    //   northEast = null;
    //   southEast = null;
    //   southWest = null;
    //   northWest = null;
    //   hasChildren = false;
    // }

    // internal void Regenerate () {
    //   if (patch)
    //     patch.dirty = true;
    // }

    // public void DestroyMesh () {
    //   if (patch)
    //     GameObject.DestroyImmediate(patch.gameObject);
    //   patch = null;
    // }

    // public void Destroy () {
    //   DestroyChildren();
    //   DestroyMesh();
    //   GameObject.DestroyImmediate(gameObject);
    // }

    // public IEnumerable<Node> Collect (List<Node> nodes = null) {
    //   nodes ??= new List<Node>();
    //   nodes.Add(this);
    //   if (northEast)
    //     northEast.Collect(nodes);
    //   if (southEast)
    //     southEast.Collect(nodes);
    //   if (southWest)
    //     southWest.Collect(nodes);
    //   if (northWest)
    //     northWest.Collect(nodes);
    //   return nodes;
    // }

    // public void ForEach (Action<Node> f) {
    //   f(this);
    //   if (northEast)
    //     northEast.ForEach(f);
    //   if (southEast)
    //     southEast.ForEach(f);
    //   if (southWest)
    //     southWest.ForEach(f);
    //   if (northWest)
    //     northWest.ForEach(f);
    // }

    // internal void SetNeighbours (Node n, Node e, Node s, Node w) {
    //   north = n;
    //   east = e;
    //   south = s;
    //   west = w;
    //   if (n)
    //     n.south = this;
    //   if (e)
    //     e.west = this;
    //   if (s)
    //     s.north = this;
    //   if (w)
    //     w.east = this;
    // }

    // internal void CreateMesh () {
    //   if (patch) {
    //     if (patch.dirty)
    //       patch.Setup(this);
    //     return;
    //   }
    //   patch = new GameObject().AddComponent<Patch>();
    //   patch.transform.SetParent(transform, false);
    //   patch.Setup(this);
    // }

    // public void UpdateVisibility () {
    //   if (patch)
    //     patch.gameObject.SetActive(!hasChildren);
    // }

    // public void Split () {
    //   if (!hasChildren) {
    //     hasChildren = true;
    //     var nn = (ne + nw) / 2;
    //     var ss = (se + sw) / 2;
    //     var ee = (ne + se) / 2;
    //     var ww = (nw + sw) / 2;
    //     var mm = (nn + ss) / 2;
    //     northWest = Node.New().Setup(world, name + "-NW", NodeType.NorthWest, this, nw, nn, ww, mm);
    //     northEast = Node.New().Setup(world, name + "-NE", NodeType.NorthEast, this, nn, ne, mm, ee);
    //     southWest = Node.New().Setup(world, name + "-SW", NodeType.SouthWest, this, ww, mm, sw, ss);
    //     southEast = Node.New().Setup(world, name + "-SE", NodeType.SouthEast, this, mm, ee, ss, se);
    //     UpdateVisibility();
    //   }
    // }

    // public void Unsplit () {
    //   if (hasChildren) {
    //     DestroyChildren();
    //     UpdateVisibility();
    //   }
    // }

    // public void UpdateChildren (Vector3[] positions) {
    //   var position = centrePoint;
    //   observerDistance = positions.Length == 0 ? 2 * world.size : positions.Min(p => Vector3.Distance(position, p));
    //   if (depth < world.minDepth
    //       || (depth < world.maxDepth
    //           && edgeLength > world.patchLengthLimit
    //           && observerDistance < world.splitMultiplier * edgeLength)) {
    //     if (!hasChildren) {
    //       // Split();
    //       world.splitQueue.EnqueueWithoutDuplicates(this, new Priority(-depth, observerDistance));
    //     } else {
    //       northWest?.UpdateChildren(positions);
    //       northEast?.UpdateChildren(positions);
    //       southWest?.UpdateChildren(positions);
    //       southEast?.UpdateChildren(positions);
    //     }
    //   } else if (hasChildren) {
    //     Unsplit();
    //   }
    // }

    // public void UpdateNeighbours () {
    //   switch (nodeType) {
    //   case NodeType.NorthWest:
    //     SetNeighbours(up.north?.southWest, up.northEast, up.southWest, up.west?.northEast);
    //     break;
    //   case NodeType.NorthEast:
    //     SetNeighbours(up.north?.southEast, up.east?.northWest, up.southEast, up.northWest);
    //     break;
    //   case NodeType.SouthWest:
    //     SetNeighbours(up.northWest, up.southEast, up.south?.northWest, up.west?.southEast);
    //     break;
    //   case NodeType.SouthEast:
    //     SetNeighbours(up.northEast, up.east?.southWest, up.south?.northEast, up.southWest);
    //     break;
    //   }
    //   northWest?.UpdateNeighbours();
    //   northEast?.UpdateNeighbours();
    //   southEast?.UpdateNeighbours();
    //   southWest?.UpdateNeighbours();
    // }

    // public void FixCracks () {
    //   var newIndexBuffer = IndexBufferSelection.Base;
    //   switch (nodeType) {
    //   case NodeType.NorthWest:
    //     if (north == null && west == null) {
    //       newIndexBuffer = IndexBufferSelection.NwCrackFix;
    //     } else if (north == null) {
    //       newIndexBuffer = IndexBufferSelection.NCrackFix;
    //     } else if (west == null) {
    //       newIndexBuffer = IndexBufferSelection.WCrackFix;
    //     }
    //     break;
    //   case NodeType.NorthEast:
    //     if (north == null && east == null) {
    //       newIndexBuffer = IndexBufferSelection.NeCrackFix;
    //     } else if (north == null) {
    //       newIndexBuffer = IndexBufferSelection.NCrackFix;
    //     } else if (east == null) {
    //       newIndexBuffer = IndexBufferSelection.ECrackFix;
    //     }
    //     break;
    //   case NodeType.SouthWest:
    //     if (south == null && west == null) {
    //       newIndexBuffer = IndexBufferSelection.SwCrackFix;
    //     } else if (south == null) {
    //       newIndexBuffer = IndexBufferSelection.SCrackFix;
    //     } else if (west == null) {
    //       newIndexBuffer = IndexBufferSelection.WCrackFix;
    //     }
    //     break;
    //   case NodeType.SouthEast:
    //     if (south == null && east == null) {
    //       newIndexBuffer = IndexBufferSelection.SeCrackFix;
    //     } else if (south == null) {
    //       newIndexBuffer = IndexBufferSelection.SCrackFix;
    //     } else if (east == null) {
    //       newIndexBuffer = IndexBufferSelection.ECrackFix;
    //     }
    //     break;
    //   }
    //   if (patch && activeIndexBuffer != newIndexBuffer)
    //     patch.dirty = true;
    //   activeIndexBuffer = newIndexBuffer;
    // }

  }

}
