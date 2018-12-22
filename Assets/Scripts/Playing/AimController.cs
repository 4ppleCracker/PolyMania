using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimController : SingletonBehaviour<AimController> {

    private int m_selectedSlice = 0;
    /// <summary>
    /// 0 indexed
    /// </summary>
    public int SelectedSlice { get { return m_selectedSlice; } private set { m_selectedSlice = value; PolyMesh.Instance.UpdateMesh(); } }
	
	// Update is called once per frame
	void Update () {
        if (Beatmap.CurrentlyLoaded != null)
        {
            Vector2 originRelative = Input.mousePosition - Camera.main.WorldToScreenPoint(PolyMesh.Instance.transform.position);
            float arctan = Mathf.Atan(originRelative.x / originRelative.y);
            float sectorDec = arctan / ((2 * Mathf.PI) / Beatmap.CurrentlyLoaded.SliceCount);

            float max = Beatmap.CurrentlyLoaded.SliceCount / 4;

            sectorDec = originRelative.x < 0 ? -sectorDec : sectorDec;
            sectorDec = sectorDec > 0 ? sectorDec : max * 2 + sectorDec + 1;
            sectorDec = originRelative.x < 0 ? Beatmap.CurrentlyLoaded.SliceCount - sectorDec : sectorDec;

            int sector = (int)sectorDec;

            SelectedSlice = sector;
        }
    }
}
