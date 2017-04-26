package com.google.android.gms.ads.internal.request;

import android.os.Parcel;
import android.os.ParcelFileDescriptor;
import android.os.ParcelFileDescriptor.AutoCloseInputStream;
import android.os.ParcelFileDescriptor.AutoCloseOutputStream;
import android.os.Parcelable;
import android.os.Parcelable.Creator;
import com.google.android.gms.ads.internal.util.client.zzb;
import com.google.android.gms.ads.internal.zzp;
import com.google.android.gms.common.internal.safeparcel.SafeParcelable;
import com.google.android.gms.internal.zzmt;
import java.io.Closeable;
import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.IOException;

public final class LargeParcelTeleporter implements SafeParcelable {
    public static final Creator<LargeParcelTeleporter> CREATOR = new zzl();
    final int mVersionCode;
    ParcelFileDescriptor zzFc;
    private Parcelable zzFd;
    private boolean zzFe;

    LargeParcelTeleporter(int versionCode, ParcelFileDescriptor parcelFileDescriptor) {
        this.mVersionCode = versionCode;
        this.zzFc = parcelFileDescriptor;
        this.zzFd = null;
        this.zzFe = true;
    }

    public LargeParcelTeleporter(SafeParcelable teleportee) {
        this.mVersionCode = 1;
        this.zzFc = null;
        this.zzFd = teleportee;
        this.zzFe = false;
    }

    public int describeContents() {
        return 0;
    }

    public void writeToParcel(Parcel dest, int flags) {
        if (this.zzFc == null) {
            Parcel obtain = Parcel.obtain();
            try {
                this.zzFd.writeToParcel(obtain, 0);
                byte[] marshall = obtain.marshall();
                this.zzFc = zzf(marshall);
            } finally {
                obtain.recycle();
            }
        }
        zzl.zza(this, dest, flags);
    }

    public <T extends SafeParcelable> T zza(Creator<T> creator) {
        if (this.zzFe) {
            if (this.zzFc == null) {
                zzb.e("File descriptor is empty, returning null.");
                return null;
            }
            Closeable dataInputStream = new DataInputStream(new AutoCloseInputStream(this.zzFc));
            try {
                byte[] bArr = new byte[dataInputStream.readInt()];
                dataInputStream.readFully(bArr, 0, bArr.length);
                zzmt.zzb(dataInputStream);
                Parcel obtain = Parcel.obtain();
                try {
                    obtain.unmarshall(bArr, 0, bArr.length);
                    obtain.setDataPosition(0);
                    this.zzFd = (SafeParcelable) creator.createFromParcel(obtain);
                    this.zzFe = false;
                } finally {
                    obtain.recycle();
                }
            } catch (Throwable e) {
                throw new IllegalStateException("Could not read from parcel file descriptor", e);
            } catch (Throwable th) {
                zzmt.zzb(dataInputStream);
            }
        }
        return (SafeParcelable) this.zzFd;
    }

    protected <T> ParcelFileDescriptor zzf(final byte[] bArr) {
        final Closeable autoCloseOutputStream;
        Throwable e;
        ParcelFileDescriptor parcelFileDescriptor = null;
        try {
            ParcelFileDescriptor[] createPipe = ParcelFileDescriptor.createPipe();
            autoCloseOutputStream = new AutoCloseOutputStream(createPipe[1]);
            try {
                new Thread(new Runnable(this) {
                    final /* synthetic */ LargeParcelTeleporter zzFh;

                    public void run() {
                        Closeable dataOutputStream;
                        Throwable e;
                        try {
                            dataOutputStream = new DataOutputStream(autoCloseOutputStream);
                            try {
                                dataOutputStream.writeInt(bArr.length);
                                dataOutputStream.write(bArr);
                                if (dataOutputStream == null) {
                                    zzmt.zzb(autoCloseOutputStream);
                                } else {
                                    zzmt.zzb(dataOutputStream);
                                }
                            } catch (IOException e2) {
                                e = e2;
                                try {
                                    zzb.zzb("Error transporting the ad response", e);
                                    zzp.zzby().zzc(e, true);
                                    if (dataOutputStream != null) {
                                        zzmt.zzb(autoCloseOutputStream);
                                    } else {
                                        zzmt.zzb(dataOutputStream);
                                    }
                                } catch (Throwable th) {
                                    e = th;
                                    if (dataOutputStream != null) {
                                        zzmt.zzb(autoCloseOutputStream);
                                    } else {
                                        zzmt.zzb(dataOutputStream);
                                    }
                                    throw e;
                                }
                            }
                        } catch (IOException e3) {
                            e = e3;
                            dataOutputStream = null;
                            zzb.zzb("Error transporting the ad response", e);
                            zzp.zzby().zzc(e, true);
                            if (dataOutputStream != null) {
                                zzmt.zzb(dataOutputStream);
                            } else {
                                zzmt.zzb(autoCloseOutputStream);
                            }
                        } catch (Throwable th2) {
                            e = th2;
                            dataOutputStream = null;
                            if (dataOutputStream != null) {
                                zzmt.zzb(dataOutputStream);
                            } else {
                                zzmt.zzb(autoCloseOutputStream);
                            }
                            throw e;
                        }
                    }
                }).start();
                return createPipe[0];
            } catch (IOException e2) {
                e = e2;
            }
        } catch (IOException e3) {
            e = e3;
            autoCloseOutputStream = parcelFileDescriptor;
            zzb.zzb("Error transporting the ad response", e);
            zzp.zzby().zzc(e, true);
            zzmt.zzb(autoCloseOutputStream);
            return parcelFileDescriptor;
        }
    }
}
