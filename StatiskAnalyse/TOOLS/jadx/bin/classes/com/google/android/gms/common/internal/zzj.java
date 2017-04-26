package com.google.android.gms.common.internal;

import android.accounts.Account;
import android.app.PendingIntent;
import android.content.ComponentName;
import android.content.Context;
import android.content.ServiceConnection;
import android.os.Bundle;
import android.os.DeadObjectException;
import android.os.Handler;
import android.os.IBinder;
import android.os.IInterface;
import android.os.Looper;
import android.os.Message;
import android.os.RemoteException;
import android.util.Log;
import com.google.android.gms.auth.GoogleAuthUtil;
import com.google.android.gms.common.ConnectionResult;
import com.google.android.gms.common.GoogleApiAvailability;
import com.google.android.gms.common.api.GoogleApiClient.ConnectionCallbacks;
import com.google.android.gms.common.api.GoogleApiClient.OnConnectionFailedListener;
import com.google.android.gms.common.api.Scope;
import gov.nist.core.Separators;
import java.io.FileDescriptor;
import java.io.PrintWriter;
import java.util.ArrayList;
import java.util.Set;
import java.util.concurrent.atomic.AtomicInteger;

public abstract class zzj<T extends IInterface> implements com.google.android.gms.common.api.Api.zzb, com.google.android.gms.common.internal.zzk.zza {
    public static final String[] zzafI = new String[]{"service_esmobile", "service_googleme"};
    private final Context mContext;
    final Handler mHandler;
    private final Account zzQd;
    private final Set<Scope> zzTm;
    private final Looper zzaaO;
    private final GoogleApiAvailability zzaaP;
    private final zzf zzabI;
    private T zzafA;
    private final ArrayList<zzc<?>> zzafB;
    private zze zzafC;
    private int zzafD;
    private final ConnectionCallbacks zzafE;
    private final OnConnectionFailedListener zzafF;
    private final int zzafG;
    protected AtomicInteger zzafH;
    private final zzl zzafx;
    private zzs zzafy;
    private com.google.android.gms.common.api.GoogleApiClient.zza zzafz;
    private final Object zzpd;

    protected abstract class zzc<TListener> {
        private TListener mListener;
        final /* synthetic */ zzj zzafK;
        private boolean zzafL = false;

        public zzc(zzj com_google_android_gms_common_internal_zzj, TListener tListener) {
            this.zzafK = com_google_android_gms_common_internal_zzj;
            this.mListener = tListener;
        }

        public void unregister() {
            zzpi();
            synchronized (this.zzafK.zzafB) {
                this.zzafK.zzafB.remove(this);
            }
        }

        protected abstract void zzpg();

        public void zzph() {
            synchronized (this) {
                Object obj = this.mListener;
                if (this.zzafL) {
                    Log.w("GmsClient", "Callback proxy " + this + " being reused. This is not safe.");
                }
            }
            if (obj != null) {
                try {
                    zzt(obj);
                } catch (RuntimeException e) {
                    zzpg();
                    throw e;
                }
            }
            zzpg();
            synchronized (this) {
                this.zzafL = true;
            }
            unregister();
        }

        public void zzpi() {
            synchronized (this) {
                this.mListener = null;
            }
        }

        protected abstract void zzt(TListener tListener);
    }

    private abstract class zza extends zzc<Boolean> {
        public final int statusCode;
        public final Bundle zzafJ;
        final /* synthetic */ zzj zzafK;

        protected zza(zzj com_google_android_gms_common_internal_zzj, int i, Bundle bundle) {
            this.zzafK = com_google_android_gms_common_internal_zzj;
            super(com_google_android_gms_common_internal_zzj, Boolean.valueOf(true));
            this.statusCode = i;
            this.zzafJ = bundle;
        }

        protected void zzc(Boolean bool) {
            PendingIntent pendingIntent = null;
            if (bool == null) {
                this.zzafK.zzb(1, null);
                return;
            }
            switch (this.statusCode) {
                case 0:
                    if (!zzpf()) {
                        this.zzafK.zzb(1, null);
                        zzh(new ConnectionResult(8, null));
                        return;
                    }
                    return;
                case 10:
                    this.zzafK.zzb(1, null);
                    throw new IllegalStateException("A fatal developer error has occurred. Check the logs for further information.");
                default:
                    this.zzafK.zzb(1, null);
                    if (this.zzafJ != null) {
                        pendingIntent = (PendingIntent) this.zzafJ.getParcelable("pendingIntent");
                    }
                    zzh(new ConnectionResult(this.statusCode, pendingIntent));
                    return;
            }
        }

        protected abstract void zzh(ConnectionResult connectionResult);

        protected abstract boolean zzpf();

        protected void zzpg() {
        }

        protected /* synthetic */ void zzt(Object obj) {
            zzc((Boolean) obj);
        }
    }

    final class zzb extends Handler {
        final /* synthetic */ zzj zzafK;

        public zzb(zzj com_google_android_gms_common_internal_zzj, Looper looper) {
            this.zzafK = com_google_android_gms_common_internal_zzj;
            super(looper);
        }

        private void zza(Message message) {
            zzc com_google_android_gms_common_internal_zzj_zzc = (zzc) message.obj;
            com_google_android_gms_common_internal_zzj_zzc.zzpg();
            com_google_android_gms_common_internal_zzj_zzc.unregister();
        }

        private boolean zzb(Message message) {
            return message.what == 2 || message.what == 1 || message.what == 5 || message.what == 6;
        }

        public void handleMessage(Message msg) {
            if (this.zzafK.zzafH.get() != msg.arg1) {
                if (zzb(msg)) {
                    zza(msg);
                }
            } else if ((msg.what == 1 || msg.what == 5 || msg.what == 6) && !this.zzafK.isConnecting()) {
                zza(msg);
            } else if (msg.what == 3) {
                ConnectionResult connectionResult = new ConnectionResult(msg.arg2, null);
                this.zzafK.zzafz.zza(connectionResult);
                this.zzafK.onConnectionFailed(connectionResult);
            } else if (msg.what == 4) {
                this.zzafK.zzb(4, null);
                if (this.zzafK.zzafE != null) {
                    this.zzafK.zzafE.onConnectionSuspended(msg.arg2);
                }
                this.zzafK.onConnectionSuspended(msg.arg2);
                this.zzafK.zza(4, 1, null);
            } else if (msg.what == 2 && !this.zzafK.isConnected()) {
                zza(msg);
            } else if (zzb(msg)) {
                ((zzc) msg.obj).zzph();
            } else {
                Log.wtf("GmsClient", "Don't know how to handle message: " + msg.what, new Exception());
            }
        }
    }

    public static final class zzd extends com.google.android.gms.common.internal.zzr.zza {
        private zzj zzafM;
        private final int zzafN;

        public zzd(zzj com_google_android_gms_common_internal_zzj, int i) {
            this.zzafM = com_google_android_gms_common_internal_zzj;
            this.zzafN = i;
        }

        private void zzpj() {
            this.zzafM = null;
        }

        public void zza(int i, IBinder iBinder, Bundle bundle) {
            zzx.zzb(this.zzafM, (Object) "onPostInitComplete can be called only once per call to getRemoteService");
            this.zzafM.zza(i, iBinder, bundle, this.zzafN);
            zzpj();
        }

        public void zzb(int i, Bundle bundle) {
            zzx.zzb(this.zzafM, (Object) "onAccountValidationComplete can be called only once per call to validateAccount");
            this.zzafM.zza(i, bundle, this.zzafN);
            zzpj();
        }
    }

    public final class zze implements ServiceConnection {
        final /* synthetic */ zzj zzafK;
        private final int zzafN;

        public zze(zzj com_google_android_gms_common_internal_zzj, int i) {
            this.zzafK = com_google_android_gms_common_internal_zzj;
            this.zzafN = i;
        }

        public void onServiceConnected(ComponentName component, IBinder binder) {
            zzx.zzb((Object) binder, (Object) "Expecting a valid IBinder");
            this.zzafK.zzafy = com.google.android.gms.common.internal.zzs.zza.zzaK(binder);
            this.zzafK.zzbF(this.zzafN);
        }

        public void onServiceDisconnected(ComponentName component) {
            this.zzafK.mHandler.sendMessage(this.zzafK.mHandler.obtainMessage(4, this.zzafN, 1));
        }
    }

    protected class zzf implements com.google.android.gms.common.api.GoogleApiClient.zza {
        final /* synthetic */ zzj zzafK;

        public zzf(zzj com_google_android_gms_common_internal_zzj) {
            this.zzafK = com_google_android_gms_common_internal_zzj;
        }

        public void zza(ConnectionResult connectionResult) {
            if (connectionResult.isSuccess()) {
                this.zzafK.zza(null, this.zzafK.zzTm);
            } else if (this.zzafK.zzafF != null) {
                this.zzafK.zzafF.onConnectionFailed(connectionResult);
            }
        }

        public void zzb(ConnectionResult connectionResult) {
            throw new IllegalStateException("Legacy GmsClient received onReportAccountValidation callback.");
        }
    }

    protected final class zzg extends zza {
        final /* synthetic */ zzj zzafK;
        public final IBinder zzafO;

        public zzg(zzj com_google_android_gms_common_internal_zzj, int i, IBinder iBinder, Bundle bundle) {
            this.zzafK = com_google_android_gms_common_internal_zzj;
            super(com_google_android_gms_common_internal_zzj, i, bundle);
            this.zzafO = iBinder;
        }

        protected void zzh(ConnectionResult connectionResult) {
            if (this.zzafK.zzafF != null) {
                this.zzafK.zzafF.onConnectionFailed(connectionResult);
            }
            this.zzafK.onConnectionFailed(connectionResult);
        }

        protected boolean zzpf() {
            try {
                String interfaceDescriptor = this.zzafO.getInterfaceDescriptor();
                if (this.zzafK.zzfL().equals(interfaceDescriptor)) {
                    IInterface zzW = this.zzafK.zzW(this.zzafO);
                    if (zzW == null || !this.zzafK.zza(2, 3, zzW)) {
                        return false;
                    }
                    Bundle zzmS = this.zzafK.zzmS();
                    if (this.zzafK.zzafE != null) {
                        this.zzafK.zzafE.onConnected(zzmS);
                    }
                    return true;
                }
                Log.e("GmsClient", "service descriptor mismatch: " + this.zzafK.zzfL() + " vs. " + interfaceDescriptor);
                return false;
            } catch (RemoteException e) {
                Log.w("GmsClient", "service probably died");
                return false;
            }
        }
    }

    protected final class zzh extends zza {
        final /* synthetic */ zzj zzafK;

        public zzh(zzj com_google_android_gms_common_internal_zzj) {
            this.zzafK = com_google_android_gms_common_internal_zzj;
            super(com_google_android_gms_common_internal_zzj, 0, null);
        }

        protected void zzh(ConnectionResult connectionResult) {
            this.zzafK.zzafz.zza(connectionResult);
            this.zzafK.onConnectionFailed(connectionResult);
        }

        protected boolean zzpf() {
            this.zzafK.zzafz.zza(ConnectionResult.zzZY);
            return true;
        }
    }

    protected final class zzi extends zza {
        final /* synthetic */ zzj zzafK;

        public zzi(zzj com_google_android_gms_common_internal_zzj, int i, Bundle bundle) {
            this.zzafK = com_google_android_gms_common_internal_zzj;
            super(com_google_android_gms_common_internal_zzj, i, bundle);
        }

        protected void zzh(ConnectionResult connectionResult) {
            this.zzafK.zzafz.zzb(connectionResult);
            this.zzafK.onConnectionFailed(connectionResult);
        }

        protected boolean zzpf() {
            this.zzafK.zzafz.zzb(ConnectionResult.zzZY);
            return true;
        }
    }

    protected zzj(Context context, Looper looper, int i, zzf com_google_android_gms_common_internal_zzf, ConnectionCallbacks connectionCallbacks, OnConnectionFailedListener onConnectionFailedListener) {
        this(context, looper, zzl.zzal(context), GoogleApiAvailability.getInstance(), i, com_google_android_gms_common_internal_zzf, (ConnectionCallbacks) zzx.zzw(connectionCallbacks), (OnConnectionFailedListener) zzx.zzw(onConnectionFailedListener));
    }

    protected zzj(Context context, Looper looper, zzl com_google_android_gms_common_internal_zzl, GoogleApiAvailability googleApiAvailability, int i, zzf com_google_android_gms_common_internal_zzf, ConnectionCallbacks connectionCallbacks, OnConnectionFailedListener onConnectionFailedListener) {
        this.zzpd = new Object();
        this.zzafB = new ArrayList();
        this.zzafD = 1;
        this.zzafH = new AtomicInteger(0);
        this.mContext = (Context) zzx.zzb((Object) context, (Object) "Context must not be null");
        this.zzaaO = (Looper) zzx.zzb((Object) looper, (Object) "Looper must not be null");
        this.zzafx = (zzl) zzx.zzb((Object) com_google_android_gms_common_internal_zzl, (Object) "Supervisor must not be null");
        this.zzaaP = (GoogleApiAvailability) zzx.zzb((Object) googleApiAvailability, (Object) "API availability must not be null");
        this.mHandler = new zzb(this, looper);
        this.zzafG = i;
        this.zzabI = (zzf) zzx.zzw(com_google_android_gms_common_internal_zzf);
        this.zzQd = com_google_android_gms_common_internal_zzf.getAccount();
        this.zzTm = zza(com_google_android_gms_common_internal_zzf.zzoL());
        this.zzafE = connectionCallbacks;
        this.zzafF = onConnectionFailedListener;
    }

    private Set<Scope> zza(Set<Scope> set) {
        Set<Scope> zzb = zzb((Set) set);
        if (zzb == null) {
            return zzb;
        }
        for (Scope contains : zzb) {
            if (!set.contains(contains)) {
                throw new IllegalStateException("Expanding scopes is not permitted, use implied scopes instead");
            }
        }
        return zzb;
    }

    private boolean zza(int i, int i2, T t) {
        boolean z;
        synchronized (this.zzpd) {
            if (this.zzafD != i) {
                z = false;
            } else {
                zzb(i2, t);
                z = true;
            }
        }
        return z;
    }

    private void zzb(int i, T t) {
        boolean z = true;
        if ((i == 3) != (t != null)) {
            z = false;
        }
        zzx.zzaa(z);
        synchronized (this.zzpd) {
            this.zzafD = i;
            this.zzafA = t;
            zzc(i, t);
            switch (i) {
                case 1:
                    zzoY();
                    break;
                case 2:
                    zzoX();
                    break;
                case 3:
                    zzoW();
                    break;
            }
        }
    }

    private void zzoX() {
        if (this.zzafC != null) {
            Log.e("GmsClient", "Calling connect() while still connected, missing disconnect() for " + zzfK());
            this.zzafx.zzb(zzfK(), this.zzafC, zzoV());
            this.zzafH.incrementAndGet();
        }
        this.zzafC = new zze(this, this.zzafH.get());
        if (!this.zzafx.zza(zzfK(), this.zzafC, zzoV())) {
            Log.e("GmsClient", "unable to connect to service: " + zzfK());
            this.mHandler.sendMessage(this.mHandler.obtainMessage(3, this.zzafH.get(), 9));
        }
    }

    private void zzoY() {
        if (this.zzafC != null) {
            this.zzafx.zzb(zzfK(), this.zzafC, zzoV());
            this.zzafC = null;
        }
    }

    public void disconnect() {
        this.zzafH.incrementAndGet();
        synchronized (this.zzafB) {
            int size = this.zzafB.size();
            for (int i = 0; i < size; i++) {
                ((zzc) this.zzafB.get(i)).zzpi();
            }
            this.zzafB.clear();
        }
        zzb(1, null);
    }

    public void dump(String prefix, FileDescriptor fd, PrintWriter writer, String[] args) {
        synchronized (this.zzpd) {
            int i = this.zzafD;
            IInterface iInterface = this.zzafA;
        }
        writer.append(prefix).append("mConnectState=");
        switch (i) {
            case 1:
                writer.print("DISCONNECTED");
                break;
            case 2:
                writer.print("CONNECTING");
                break;
            case 3:
                writer.print("CONNECTED");
                break;
            case 4:
                writer.print("DISCONNECTING");
                break;
            default:
                writer.print("UNKNOWN");
                break;
        }
        writer.append(" mService=");
        if (iInterface == null) {
            writer.println("null");
        } else {
            writer.append(zzfL()).append(Separators.AT).println(Integer.toHexString(System.identityHashCode(iInterface.asBinder())));
        }
    }

    public final Context getContext() {
        return this.mContext;
    }

    public final Looper getLooper() {
        return this.zzaaO;
    }

    public boolean isConnected() {
        boolean z;
        synchronized (this.zzpd) {
            z = this.zzafD == 3;
        }
        return z;
    }

    public boolean isConnecting() {
        boolean z;
        synchronized (this.zzpd) {
            z = this.zzafD == 2;
        }
        return z;
    }

    protected void onConnectionFailed(ConnectionResult result) {
    }

    protected void onConnectionSuspended(int cause) {
    }

    protected abstract T zzW(IBinder iBinder);

    protected void zza(int i, Bundle bundle, int i2) {
        this.mHandler.sendMessage(this.mHandler.obtainMessage(5, i2, -1, new zzi(this, i, bundle)));
    }

    protected void zza(int i, IBinder iBinder, Bundle bundle, int i2) {
        this.mHandler.sendMessage(this.mHandler.obtainMessage(1, i2, -1, new zzg(this, i, iBinder, bundle)));
    }

    public void zza(com.google.android.gms.common.api.GoogleApiClient.zza com_google_android_gms_common_api_GoogleApiClient_zza) {
        this.zzafz = (com.google.android.gms.common.api.GoogleApiClient.zza) zzx.zzb((Object) com_google_android_gms_common_api_GoogleApiClient_zza, (Object) "Connection progress callbacks cannot be null.");
        zzb(2, null);
    }

    public void zza(zzp com_google_android_gms_common_internal_zzp) {
        try {
            this.zzafy.zza(new zzd(this, this.zzafH.get()), new ValidateAccountRequest(com_google_android_gms_common_internal_zzp, (Scope[]) this.zzTm.toArray(new Scope[this.zzTm.size()]), this.mContext.getPackageName(), zzpd()));
        } catch (DeadObjectException e) {
            Log.w("GmsClient", "service died");
            zzbE(1);
        } catch (Throwable e2) {
            Log.w("GmsClient", "Remote exception occurred", e2);
        }
    }

    public void zza(zzp com_google_android_gms_common_internal_zzp, Set<Scope> set) {
        try {
            GetServiceRequest zzg = new GetServiceRequest(this.zzafG).zzcl(this.mContext.getPackageName()).zzg(zzly());
            if (set != null) {
                zzg.zzd(set);
            }
            if (zzlN()) {
                zzg.zzc(zzoI()).zzc(com_google_android_gms_common_internal_zzp);
            } else if (zzpe()) {
                zzg.zzc(this.zzQd);
            }
            this.zzafy.zza(new zzd(this, this.zzafH.get()), zzg);
        } catch (DeadObjectException e) {
            Log.w("GmsClient", "service died");
            zzbE(1);
        } catch (Throwable e2) {
            Log.w("GmsClient", "Remote exception occurred", e2);
        }
    }

    protected Set<Scope> zzb(Set<Scope> set) {
        return set;
    }

    public void zzbE(int i) {
        this.mHandler.sendMessage(this.mHandler.obtainMessage(4, this.zzafH.get(), i));
    }

    protected void zzbF(int i) {
        this.mHandler.sendMessage(this.mHandler.obtainMessage(6, i, -1, new zzh(this)));
    }

    protected void zzc(int i, T t) {
    }

    protected abstract String zzfK();

    protected abstract String zzfL();

    public boolean zzlN() {
        return false;
    }

    protected Bundle zzly() {
        return new Bundle();
    }

    public Bundle zzmS() {
        return null;
    }

    public IBinder zznz() {
        return this.zzafy == null ? null : this.zzafy.asBinder();
    }

    public final Account zzoI() {
        return this.zzQd != null ? this.zzQd : new Account("<<default account>>", GoogleAuthUtil.GOOGLE_ACCOUNT_TYPE);
    }

    protected final String zzoV() {
        return this.zzabI.zzoO();
    }

    protected void zzoW() {
    }

    public void zzoZ() {
        int isGooglePlayServicesAvailable = this.zzaaP.isGooglePlayServicesAvailable(this.mContext);
        if (isGooglePlayServicesAvailable != 0) {
            zzb(1, null);
            this.zzafz = new zzf(this);
            this.mHandler.sendMessage(this.mHandler.obtainMessage(3, this.zzafH.get(), isGooglePlayServicesAvailable));
            return;
        }
        zza(new zzf(this));
    }

    protected final zzf zzpa() {
        return this.zzabI;
    }

    protected final void zzpb() {
        if (!isConnected()) {
            throw new IllegalStateException("Not connected. Call connect() and wait for onConnected() to be called.");
        }
    }

    public final T zzpc() throws DeadObjectException {
        T t;
        synchronized (this.zzpd) {
            if (this.zzafD == 4) {
                throw new DeadObjectException();
            }
            zzpb();
            zzx.zza(this.zzafA != null, (Object) "Client is connected but service is null");
            t = this.zzafA;
        }
        return t;
    }

    protected Bundle zzpd() {
        return null;
    }

    public boolean zzpe() {
        return false;
    }
}
