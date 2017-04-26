package com.google.android.gms.ads.internal.overlay;

import android.os.Parcel;
import android.os.Parcelable.Creator;
import com.google.android.gms.common.internal.safeparcel.zza;

public class zzb implements Creator<AdLauncherIntentInfoParcel> {
    static void zza(AdLauncherIntentInfoParcel adLauncherIntentInfoParcel, Parcel parcel, int i) {
        int zzaq = com.google.android.gms.common.internal.safeparcel.zzb.zzaq(parcel);
        com.google.android.gms.common.internal.safeparcel.zzb.zzc(parcel, 1, adLauncherIntentInfoParcel.versionCode);
        com.google.android.gms.common.internal.safeparcel.zzb.zza(parcel, 2, adLauncherIntentInfoParcel.intentAction, false);
        com.google.android.gms.common.internal.safeparcel.zzb.zza(parcel, 3, adLauncherIntentInfoParcel.url, false);
        com.google.android.gms.common.internal.safeparcel.zzb.zza(parcel, 4, adLauncherIntentInfoParcel.mimeType, false);
        com.google.android.gms.common.internal.safeparcel.zzb.zza(parcel, 5, adLauncherIntentInfoParcel.packageName, false);
        com.google.android.gms.common.internal.safeparcel.zzb.zza(parcel, 6, adLauncherIntentInfoParcel.zzAL, false);
        com.google.android.gms.common.internal.safeparcel.zzb.zza(parcel, 7, adLauncherIntentInfoParcel.zzAM, false);
        com.google.android.gms.common.internal.safeparcel.zzb.zza(parcel, 8, adLauncherIntentInfoParcel.zzAN, false);
        com.google.android.gms.common.internal.safeparcel.zzb.zzI(parcel, zzaq);
    }

    public /* synthetic */ Object createFromParcel(Parcel x0) {
        return zzf(x0);
    }

    public /* synthetic */ Object[] newArray(int x0) {
        return zzs(x0);
    }

    public AdLauncherIntentInfoParcel zzf(Parcel parcel) {
        String str = null;
        int zzap = zza.zzap(parcel);
        int i = 0;
        String str2 = null;
        String str3 = null;
        String str4 = null;
        String str5 = null;
        String str6 = null;
        String str7 = null;
        while (parcel.dataPosition() < zzap) {
            int zzao = zza.zzao(parcel);
            switch (zza.zzbM(zzao)) {
                case 1:
                    i = zza.zzg(parcel, zzao);
                    break;
                case 2:
                    str7 = zza.zzp(parcel, zzao);
                    break;
                case 3:
                    str6 = zza.zzp(parcel, zzao);
                    break;
                case 4:
                    str5 = zza.zzp(parcel, zzao);
                    break;
                case 5:
                    str4 = zza.zzp(parcel, zzao);
                    break;
                case 6:
                    str3 = zza.zzp(parcel, zzao);
                    break;
                case 7:
                    str2 = zza.zzp(parcel, zzao);
                    break;
                case 8:
                    str = zza.zzp(parcel, zzao);
                    break;
                default:
                    zza.zzb(parcel, zzao);
                    break;
            }
        }
        if (parcel.dataPosition() == zzap) {
            return new AdLauncherIntentInfoParcel(i, str7, str6, str5, str4, str3, str2, str);
        }
        throw new zza.zza("Overread allowed size end=" + zzap, parcel);
    }

    public AdLauncherIntentInfoParcel[] zzs(int i) {
        return new AdLauncherIntentInfoParcel[i];
    }
}
