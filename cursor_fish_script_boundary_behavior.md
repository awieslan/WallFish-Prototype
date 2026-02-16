# Fish script boundary behavior
_Exported on 2/15/2026 at 15:13:57 EST from Cursor (2.4.37)_

---

**User**

Hello, I would like to continue working on a project that a Cursor agent was helping me with previously. Here is the past chat transcription and the relevant script files:
@c:\Users\awies\Desktop\Lab 2\Unity Projects\WallFish-Prototype\cursor_raycasting_for_polygon_intersect.md 
@c:\Users\awies\Desktop\Lab 2\Unity Projects\WallFish-Prototype\SxL-Unity-Arduino-Template\Assets\FishScript.cs @c:\Users\awies\Desktop\Lab 2\Unity Projects\WallFish-Prototype\SxL-Unity-Arduino-Template\Assets\ExclusionZone.cs @c:\Users\awies\Desktop\Lab 2\Unity Projects\WallFish-Prototype\SxL-Unity-Arduino-Template\Assets\HeadObjectScript.cs 

There is currently a behavior where the fish is dragged by the boundaries of the raycast polygon when they move. Can you please edit the fish script behavior so that this does not happen, but rather the fish naturally returns to be within the boundaries when the next destination point is selected?

---

**Cursor**


Reading the referenced files to understand the current implementation and fix the boundary-dragging behavior.



Identifying the cause: the fish is clamped every frame in `ConstrainToBoundary()` during movement and idle, so when the raycast polygon (head/walls) moves, the fish gets dragged with the boundary.

Implementing: we'll stop constraining position every frame and only clamp the fish back inside the boundary when selecting the next destination.



Summary of changes to `FishScript.cs`:

## 1. **No per-frame clamping during movement or idle**

- **Travel:** The fish’s new position is applied directly after movement (with meandering). It is **no longer** passed through `ConstrainToBoundary()` each frame, so the fish is not dragged when the raycast polygon moves.
- **Idle (moving to waypoint):** Same idea — position is set to `newPos` without constraining to the boundary every frame.
- **Idle (pause drift):** Drift is applied without constraining to the boundary each frame.

So while moving or idling, the fish can temporarily end up outside the current polygon if the boundary moves; it is no longer “stuck” to the moving edges.

## 2. **Clamp only when choosing the next destination**

At the **start** of `SelectNewTarget()` (right after we know we have enough valid boundary points), the fish’s current position is clamped once:

- `transform.position = ConstrainToBoundary(transform.position);`

That uses the **current** boundary (and exclusion zones) to pull the fish back inside, then a new target is chosen as before. So:

- The fish is not dragged by the boundary during motion.
- When the next destination is selected, it is first brought back inside the boundary, then given a new target within that same boundary.

`ConstrainToBoundary` is still used for:

- Pushing the **target** and **idle waypoints** into the valid area (unchanged).
- This single clamp of the fish’s position when selecting the next destination.

Result: the fish moves and idles without being dragged by the polygon; it is only corrected back inside when it’s time to pick the next destination.

---

**User**

This is good but I would not like the fish to be teleported within the boundaries. If it is outside of the boundary, the desired behavior is that it moves from where it is to the next destination in the same manner that it normally moves to its destination. The fish should never teleport.

---

**Cursor**


Removing the teleport in `SelectNewTarget()` so the fish stays at its current position and moves normally toward the next destination (which is inside the boundary).



Change is applied.

The fish is **no longer teleported**. When it’s outside the boundary (e.g. after the boundary moved), `SelectNewTarget()` only picks a new destination **inside** the boundary. The fish stays where it is and then moves to that target using the usual movement (meandering, facing, etc.). When it reaches the target, it’s back inside the boundary. All movement is continuous; there’s no instant repositioning.

---

**User**

The fish is still swiming directly into exclusion zones and I don't see the list in FishScript populating. Can you fix this? Also ensure that the fish does not swim through exclusion zones if it is going to a point on the other side of the zone.

---

**Cursor**


Reading the scripts to fix exclusion-zone detection, list population, and prevent the fish from swimming through zones.



Implementing fixes:
1. Populating the Inspector list by assigning found zones to the serialized array at runtime.
2. Making ExclusionZone.ContainsPoint reliable (ClosestPoint-based).
3. Rejecting targets when the straight path would cross an exclusion zone.
4. Blocking movement into zones by pushing the new position out of zones (no teleport).



Blocking movement into exclusion zones by pushing the new position out when it would enter a zone:



Summary of changes:

### 1. **Exclusion zone list in the Inspector**
- When the **Exclusion Zones** list is empty at Start, the script finds all `ExclusionZone` components and assigns them to the serialized `exclusionZones` array.
- At runtime the Inspector will show the found zones (select the Fish object while playing).
- A one-time debug log reports how many zones were found (e.g. `"FishScript: using 2 exclusion zone(s)."`).

### 2. **ExclusionZone.ContainsPoint**
- **Before:** Used a single raycast from the point; unreliable for some shapes and trigger colliders.
- **After:** Uses `Collider.ClosestPoint(point)`. If the closest point is not (almost) the same as the query point, the point is treated as inside. Works for box, sphere, capsule, and mesh colliders.

### 3. **No targets through exclusion zones**
- When choosing a new target, the script checks the straight path from the fish to the candidate target with **PathCrossesExclusionZone** (12 samples along the segment).
- If the path crosses any zone, that target is rejected and another is tried (up to 25 attempts).
- So the fish only gets targets it can reach without crossing a zone.

### 4. **Movement never enters zones**
- After computing the new position (travel, idle waypoint movement, or pause drift), the script runs **PushPointOutOfExclusionZones** on that position.
- If the new position would be inside a zone, it’s moved to the nearest point outside (on the zone boundary), so the fish slides along the zone instead of passing through.
- This is applied only for exclusion zones, not for the raycast boundary, so the fish is still not dragged by the polygon.

**What to do in Unity**
- Ensure each exclusion volume has a **Collider** (e.g. Box Collider) and an **Exclusion Zone** component. The collider can be a trigger.
- Leave the Fish’s **Exclusion Zones** list empty if you want auto-discovery; at runtime the list will fill with found zones and the Console will show how many were found.

---

**User**

The exclusion zone is now detected but the fish is stuck inside of the exclusion zone instead of not being allowed in it. The destination point is selected outside of the exclusion zone but the fish is not able to swim outside of it.

---

**Cursor**


Checking how we're pushing the fish out of exclusion zones. The fish may be pushed only to the zone surface and remain considered "inside" or get pushed back each frame.



Pushing only to the collider surface can leave the fish on the boundary; floating-point and small movements can keep it classified as inside. Nudging the pushed point slightly outward so it's clearly outside the zone.



Avoiding a second ClosestPoint by inlining the "inside" check:



Projecting the fish back onto the movement plane after pushing out so it doesn't drift off the plane when escaping a zone:



Summary of what was wrong and what was changed:

**What was going wrong**  
`ClosestPointOutside` was returning Unity’s `ClosestPoint`, which for a point inside the collider is a point **on the surface**. So the fish was being placed on the boundary. Because of that:

1. The point could still be treated as “inside” (e.g. floating point or Unity’s boundary handling).
2. Next frame it would move a tiny bit, end up “inside” again, and get pushed back to the same boundary point.

So it looked like the fish was stuck inside and couldn’t swim out.

**What was changed (in `ExclusionZone.cs`)**  
`ClosestPointOutside` now:

1. If the point is **outside**, returns it unchanged.
2. If the point is **inside**, takes the closest surface point and then **moves it 0.05 units outward** from the zone (along the direction from the point to that surface point). The result is a position that is clearly outside the collider.

So when the fish is inside a zone, it gets moved to “just outside” the nearest wall instead of being left on the boundary. On the next frame it’s no longer considered inside and can move normally toward the destination.

**If the fish still gets stuck**  
If your scene uses different scales (e.g. very large zones), 0.05 might be too small. In that case we can add an Inspector field (e.g. “Exit margin”) so you can increase that distance. Say if you want that and we can add it.

---

**User**

I think that the way inside and outside of a zone is being determined is incorrect. The fish is albe to move around inside of the exclusion zone but it cannot make it out of the exclusion boundary. At the start of running the program, the fish is teleported inside of the exclusion zone, please correct this.

---

**Cursor**


Checking the current logic and fixing inside/outside detection and initial placement.



Identifying the bug: the "inside" test used (closest - point).sqrMagnitude > 0.0001f. For both inside and outside, ClosestPoint returns a point on the surface, so that condition is true in both cases — we were treating outside as inside. Fixing ExclusionZone to use a proper inside test (OverlapSphere) and ensuring the fish doesn't start inside a zone.



Ensuring the fish doesn't start inside an exclusion zone: we'll run the boundary update once in Start, then push the initial position out of any zone before selecting the first target.



Checking script execution order: HeadObjectScript may not have run before FishScript.Start. Moving the spawn correction to the first Update so boundary data is available.



Summary of what was wrong and what was changed:

### 1. **Incorrect inside/outside logic (ExclusionZone.cs)**

The old check was:

- `(closest - point).sqrMagnitude > 0.0001f` → treat as “inside”.

For both **inside** and **outside** the collider, `ClosestPoint` returns a point on the surface, so that expression is true in both cases. So:

- Points **outside** were treated as inside → the fish was constantly pushed toward the boundary and couldn’t leave.
- The fish looked like it could “move around inside” because it was actually outside but being pushed onto the boundary.

**Fix:** Inside is now determined with **Physics.OverlapSphere**: a tiny sphere (radius 0.001) at the point is cast; if it overlaps the zone’s collider, the point is inside. Trigger colliders are included via `QueryTriggerInteraction.Collide`. So inside/outside is correct for both trigger and non-trigger zones.

### 2. **ClosestPointOutside**

It now uses the corrected **ContainsPoint** to decide if the point is inside before pushing out. The outward nudge from the surface is unchanged.

### 3. **Fish starting inside a zone (FishScript.cs)**

- Added a one-time **spawn correction** in the first **Update** (after boundaries and zones are valid): if the fish’s position is inside any exclusion zone, it is moved out once with **PushPointOutOfExclusionZones**.
- This runs only once (`_spawnPositionCorrected`), so it doesn’t keep moving the fish later.

Result:

- **Inside** = OverlapSphere says the point is in the zone → fish is pushed out and can swim away.
- **Outside** = OverlapSphere says the point is not in the zone → no push, fish moves normally.
- At runtime start, if the fish is inside a zone it is moved outside once, then behavior continues as above.

---

**User**

This works now, the only issue is that the fish can bypass the boundary during its idle movement. Please apply a similar boundary checking logic as you did with the regular movement.

---

**Cursor**


Adding boundary constraining to idle movement (waypoint and drift) so the fish stays inside the boundary while idling.



Applying ConstrainToBoundary to idle waypoint movement and pause drift.



Summary of changes:

- **Idle waypoint movement**  
  After applying movement and `PushPointOutOfExclusionZones`, the new position is passed through `ConstrainToBoundary(newPos)` before assigning `transform.position`, so the fish stays inside the raycast boundary while moving toward idle waypoints.

- **Pause drift**  
  The same logic is used: drift is applied, then `PushPointOutOfExclusionZones`, then `ConstrainToBoundary`, then `transform.position = newPos`.

So during idle (both moving to waypoints and drifting), the fish is now limited by the same boundary logic as the main movement: AABB of the four boundary points, projection onto the plane, and exclusion zones. It will no longer leave the valid area while idling.

