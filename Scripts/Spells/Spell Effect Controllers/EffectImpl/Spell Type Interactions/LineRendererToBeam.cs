using UnityEngine;
using System.Collections;

public class LineRendererToBeam : SpellEffect
{
    public BeamMotor beamMotor;
    private LineRenderer lineRenderer;
    public int lineCount = 2;

    /// <summary>
    /// How much each point should be apart from each other to evenly distribute them given
    /// the line count
    /// </summary>
    private float DistributionIndex
    {
        get { return Vector3.Distance(effectSetting.spell.SpellStartTransform.position, beamMotor.BeamLocation) / (lineCount - 1); }
    }

    protected override void Start()
    {
        base.Start();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetVertexCount(lineCount);

        UpdateLinePositions();
    }

    protected override void UpdateSpell()
    {
        base.UpdateSpell();

        UpdateLinePositions();
    }

    private void UpdateLinePositions()
    {
        for (int i = 0; i < lineCount; i++)
            Debug.DrawRay(transform.position + (beamMotor.BeamDirection) * (DistributionIndex * i), Vector3.up * 5f, Color.red);

        for (int i = 0; i < lineCount; i++)
        {
            Vector3 offset = Vector3.zero;
             if (i > 0 && i < lineCount - 1)
             offset = new Vector3(Random.Range(-2, 2), 0, 0);

            lineRenderer.SetPosition(i, transform.position + (beamMotor.BeamDirection) * (DistributionIndex * i) + offset);
        }
    }
}
