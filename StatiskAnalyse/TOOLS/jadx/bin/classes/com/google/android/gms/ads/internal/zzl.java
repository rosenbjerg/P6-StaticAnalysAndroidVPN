package com.google.android.gms.ads.internal;

import android.os.Parcel;
import android.os.Parcelable.Creator;
import com.google.android.gms.common.internal.safeparcel.zza;
import com.google.android.gms.common.internal.safeparcel.zzb;

public class zzl implements Creator<InterstitialAdParameterParcel> {
    static void zza(InterstitialAdParameterParcel interstitialAdParameterParcel, Parcel parcel, int i) {
        int zzaq = zzb.zzaq(parcel);
        zzb.zzc(parcel, 1, interstitialAdParameterParcel.versionCode);
        zzb.zza(parcel, 2, interstitialAdParameterParcel.zzpt);
        zzb.zza(parcel, 3, interstitialAdParameterParcel.zzpu);
        zzb.zza(parcel, 4, interstitialAdParameterParcel.zzpv, false);
        zzb.zza(parcel, 5, interstitialAdParameterParcel.zzpw);
        zzb.zza(parcel, 6, interstitialAdParameterParcel.zzpx);
        zzb.zzI(parcel, zzaq);
    }

    public /* synthetic */ Object createFromParcel(Parcel x0) {
        return zza(x0);
    }

    public /* synthetic */ Object[] newArray(int x0) {
        return zzf(x0);
    }

    public InterstitialAdParameterParcel zza(Parcel parcel) {
        boolean z = false;
        int zzap = zza.zzap(parcel);
        String str = null;
        float f = 0.0f;
        boolean z2 = false;
        boolean z3 = false;
        int i = 0;
        while (parcel.dataPosition() < zzap) {
            int zzao = zza.zzao(parcel);
            switch (zza.zzbM(zzao)) {
                case 1:
                    i = zza.zzg(parcel, zzao);
                    break;
                case 2:
                    z3 = zza.zzc(parcel, zzao);
                    break;
                case 3:
                    z2 = zza.zzc(parcel, zzao);
                    break;
                case 4:
                    str = zza.zzp(parcel, zzao);
                    break;
                case 5:
                    z = zza.zzc(parcel, zzao);
                    break;
                case 6:
                    f = zza.zzl(parcel, zzao);
                    break;
                default:
                    zza.zzb(parcel, zzao);
                    break;
            }
        }
        if (parcel.dataPosition() == zzap) {
            return new InterstitialAdParameterParcel(i, z3, z2, str, z, f);
        }
        throw new zza.zza("Overread allowed size end=" + zzap, parcel);
    }

    public InterstitialAdParameterParcel[] zzf(int i) {
        return new InterstitialAdParameterParcel[i];
    }
}
