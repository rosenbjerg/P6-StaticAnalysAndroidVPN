package com.google.android.gms.ads.internal.request;

import android.os.Parcel;
import android.os.ParcelFileDescriptor;
import android.os.Parcelable.Creator;
import com.google.android.gms.common.internal.safeparcel.zza;
import com.google.android.gms.common.internal.safeparcel.zzb;

public class zzl implements Creator<LargeParcelTeleporter> {
    static void zza(LargeParcelTeleporter largeParcelTeleporter, Parcel parcel, int i) {
        int zzaq = zzb.zzaq(parcel);
        zzb.zzc(parcel, 1, largeParcelTeleporter.mVersionCode);
        zzb.zza(parcel, 2, largeParcelTeleporter.zzFc, i, false);
        zzb.zzI(parcel, zzaq);
    }

    public /* synthetic */ Object createFromParcel(Parcel x0) {
        return zzl(x0);
    }

    public /* synthetic */ Object[] newArray(int x0) {
        return zzG(x0);
    }

    public LargeParcelTeleporter[] zzG(int i) {
        return new LargeParcelTeleporter[i];
    }

    public LargeParcelTeleporter zzl(Parcel parcel) {
        int zzap = zza.zzap(parcel);
        int i = 0;
        ParcelFileDescriptor parcelFileDescriptor = null;
        while (parcel.dataPosition() < zzap) {
            int zzao = zza.zzao(parcel);
            switch (zza.zzbM(zzao)) {
                case 1:
                    i = zza.zzg(parcel, zzao);
                    break;
                case 2:
                    parcelFileDescriptor = (ParcelFileDescriptor) zza.zza(parcel, zzao, ParcelFileDescriptor.CREATOR);
                    break;
                default:
                    zza.zzb(parcel, zzao);
                    break;
            }
        }
        if (parcel.dataPosition() == zzap) {
            return new LargeParcelTeleporter(i, parcelFileDescriptor);
        }
        throw new zza.zza("Overread allowed size end=" + zzap, parcel);
    }
}
