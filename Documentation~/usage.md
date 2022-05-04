////////////////////////////////////////

elírt window layer
	Add to existing (if there is one)
	Create (if enabled)
	Default (if specified)
	throw

////////////////////////////////////////
void ShowErrorMessage()
{
	messagePanel.Text = errorMessage;
	WindowManager.Show("Modal/Error");
}

void ShowGameplayOptions()
{
	var wnd = WindowManager.Show<OptionsWindow>("Pause/Options");
	wnd.SelectTab(3);
}

void DisableHUD()
{
	WindowManager.GetLayer("HUD").Disable();
}

void ShowPauseMenu()
{
	Game.Pause();
	WindowManager.GetLayer("HUD").Disable();
	WindowManager.Show("Pause/Pause");
}

void StoredWindow()
{
	var announcementWindow = WindowManager.GetWindow("announcement");
	
	announcementWindow.SetText("You have reached level 10!");
	announcementWindow.Show();
}