package com.google.android.gms.ads.internal.formats;

import android.graphics.Color;
import android.graphics.drawable.Drawable;
import com.baidu.location.b.g;
import java.util.List;

public class zza {
    private static final int zzwa = Color.rgb(12, 174, g.n);
    private static final int zzwb = Color.rgb(g.c, g.c, g.c);
    static final int zzwc = zzwb;
    static final int zzwd = zzwa;
    private final int mTextColor;
    private final String zzwe;
    private final List<Drawable> zzwf;
    private final int zzwg;
    private final int zzwh;
    private final int zzwi;

    public zza(String str, List<Drawable> list, Integer num, Integer num2, Integer num3, int i) {
        this.zzwe = str;
        this.zzwf = list;
        this.zzwg = num != null ? num.intValue() : zzwc;
        this.mTextColor = num2 != null ? num2.intValue() : zzwd;
        this.zzwh = num3 != null ? num3.intValue() : 12;
        this.zzwi = i;
    }

    public int getBackgroundColor() {
        return this.zzwg;
    }

    public String getText() {
        return this.zzwe;
    }

    public int getTextColor() {
        return this.mTextColor;
    }

    public int getTextSize() {
        return this.zzwh;
    }

    public List<Drawable> zzds() {
        return this.zzwf;
    }

    public int zzdt() {
        return this.zzwi;
    }
}
