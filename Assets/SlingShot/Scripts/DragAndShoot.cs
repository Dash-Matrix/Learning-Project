using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class DragAndShoot : MonoBehaviour
{
    private Vector3 mousePressDownPos; // Position of the mouse when clicked
    private Vector3 mouseReleasePos; // Position of the mouse when released
    private Rigidbody rb; // Rigidbody component of the object
    private bool isShoot; // Flag to check if the object has been shot

    private float forceMultiplier = 3; // Multiplier to scale the force applied to the object
    public LineRenderer lineRenderer; // Reference to the LineRenderer component used to draw the trajectory
    public int trajectoryResolution = 30; // Number of points used to represent the trajectory

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component attached to the object
        lineRenderer.positionCount = 0; // Initialize LineRenderer with no points
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && !isShoot) // Check if the left mouse button is held down and the object has not been shot
        {
            Vector3 currentMousePos = Input.mousePosition; // Get the current position of the mouse
            Vector3 force = (mousePressDownPos - currentMousePos); // Calculate the force vector based on mouse drag distance
            force = new Vector3(force.x, force.y, force.y); // Adjust the force direction to ensure proper application

            VisualizeTrajectory(force); // Update the trajectory visualization based on the current force
        }
    }

    void OnMouseDown()
    {
        if (isShoot)
            return; // Do nothing if the object has already been shot

        mousePressDownPos = Input.mousePosition; // Record the position of the mouse when the object is clicked
        lineRenderer.positionCount = 0; // Clear the LineRenderer to prepare for new trajectory visualization
    }

    void OnMouseUp()
    {
        if (isShoot)
            return; // Do nothing if the object has already been shot

        mouseReleasePos = Input.mousePosition; // Record the position of the mouse when the click is released
        Vector3 force = mousePressDownPos - mouseReleasePos; // Calculate the force vector based on mouse drag distance
        force = new Vector3(force.x, force.y, force.y); // Adjust the force direction to ensure proper application
        Shoot(force); // Apply the calculated force to shoot the object
    }

    void Shoot(Vector3 force)
    {
        rb.velocity = Vector3.zero; // Reset the Rigidbody's velocity to ensure consistent force application
        rb.AddForce(force * forceMultiplier); // Apply the calculated force to the Rigidbody with the force multiplier
        isShoot = true; // Set the flag to indicate that the object has been shot
        lineRenderer.positionCount = 0; // Clear the LineRenderer after shooting
    }

    void VisualizeTrajectory(Vector3 force)
    {
        Vector3[] trajectoryPoints = CalculateTrajectoryPoints(force); // Calculate the trajectory points based on the given force
        lineRenderer.positionCount = trajectoryPoints.Length; // Set the number of points in the LineRenderer
        lineRenderer.SetPositions(trajectoryPoints); // Set the positions of the points in the LineRenderer
    }

    Vector3[] CalculateTrajectoryPoints(Vector3 force)
    {
        List<Vector3> points = new List<Vector3>(); // List to store the trajectory points
        Vector3 start = transform.position; // Starting position of the object
        Vector3 velocity = force * forceMultiplier / rb.mass; // Initial velocity of the object based on the applied force
        float timeStep = 0.1f; // Time interval between each trajectory point

        for (int i = 0; i < trajectoryResolution; i++)
        {
            float t = i * timeStep; // Time at each point
            Vector3 point = start + t * velocity / 50; // Calculate the position based on the velocity
            point.y += 0.5f * Physics.gravity.y * t * t; // Apply the effect of gravity on the trajectory
            points.Add(point); // Add the calculated point to the list

            // Debugging log to verify the calculated points
            Debug.Log($"Point {i}: {point}");

            // Optional: Stop calculating points if the point goes below the ground level
            if (point.y < 0)
                break;
        }

        return points.ToArray(); // Return the calculated trajectory points as an array
    }
}
