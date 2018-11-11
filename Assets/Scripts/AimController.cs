using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimController : SingletonBehaviour<AimController> {

    public int SelectedSlice { get; private set; }
	
	// Update is called once per frame
	void Update () {
        //Do some mathy stuff ??
        Vector2 originRelative = Input.mousePosition - Camera.main.WorldToScreenPoint(PolyMesh.Instance.transform.position);
        float arctan = Mathf.Atan2(originRelative.x, originRelative.y);
        float sectorDec = arctan / ((2 * Mathf.PI) / Beatmap.CurrentlyLoaded.SliceCount);
        int sector = (sectorDec > 0) ? (int)sectorDec : (Beatmap.CurrentlyLoaded.SliceCount + (int)sectorDec - 1);
        if(SelectedSlice != sector)
        {
            PolyMesh.Instance.UpdateMesh();
        }
        SelectedSlice = sector;
    }
}
