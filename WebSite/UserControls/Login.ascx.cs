﻿using System;
using Entities;
using Ra.Widgets;

public partial class UserControls_Login : System.Web.UI.UserControl
{
    public event EventHandler LoggedIn;

    private bool _wasVisibleInLoad;

    public void ShowLogin()
    {
        login.Visible = true;
        lblErr.Text = "";
    }

    protected void CloseLogin(object sender, EventArgs e)
    {
        login.Visible = false;
    }

    protected override void OnLoad(EventArgs e)
    {
        _wasVisibleInLoad = login.Visible;
        base.OnLoad(e);
    }

    protected void login_Click(object sender, EventArgs e)
    {
        openIdTxt.LogOn();
    }

    protected void btnNoOpenId_Click(object sender, EventArgs e)
    {
        nativeWrapper.Visible = true;
        username.Text = "username";
        new EffectFadeOut(openIdWrapper, 300)
        .ChainThese(new EffectFadeIn(nativeWrapper, 300)
            .ChainThese(new EffectFocusAndSelect(username)))
        .Render();
    }

    protected void useOpenID_Click(object sender, EventArgs e)
    {
        openIdTxt.Text = "user.someOpenId.com";
        new EffectFadeOut(nativeWrapper, 300)
        .ChainThese(new EffectFadeIn(openIdWrapper, 300)
            .ChainThese(new EffectFocusAndSelect(openIdTxt.FindControl("wrappedTextBox"))))
        .Render();
    }

    protected void openIdTxt_LoggedIn(object sender, DotNetOpenId.RelyingParty.OpenIdEventArgs e)
    {
        if (e.Response.ClaimedIdentifier != null && 
            e.Response.Status == DotNetOpenId.RelyingParty.AuthenticationStatus.Authenticated)
        {
            string userNameTxt = e.Response.ClaimedIdentifier;
            Operator.LoginOpenID(userNameTxt);

            login.Visible = false;
            if (LoggedIn != null)
                LoggedIn(this, new EventArgs());
        }
    }

    protected override void OnPreRender(EventArgs e)
    {
        if (login.Visible && !_wasVisibleInLoad)
        {
            if (nativeWrapper.Style["display"] != "none")
            {
                username.Text = "username";
                password.Text = "";
                username.Select();
                username.Focus();
            }
            else
            {
                openIdTxt.Text = "user.someOpenId.com";
                new EffectFocusAndSelect(openIdTxt.FindControl("wrappedTextBox")).Render();
            }
        }
        base.OnPreRender(e);
    }

    protected void loginBtn_Click(object sender, EventArgs e)
    {
        if (Operator.Login(username.Text, password.Text, !publicTerminale.Checked))
        {
            login.Visible = false;
            if (LoggedIn != null)
                LoggedIn(this, new EventArgs());
        }
        else
        {
            lblErr.Text = "Username/password combination was wrong...";
            password.Text = "";
            username.Select();
            username.Focus();
        }
    }
}
