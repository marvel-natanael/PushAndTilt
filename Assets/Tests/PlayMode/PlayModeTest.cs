using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayModeTest
{
    //Load the scene to test
    [OneTimeSetUp]
    public void setUp()
    {
        SceneManager.LoadScene("Menu");
    }

    [UnityTest]
    public IEnumerator JoinOnlineMatchTest()
    {
        //1.
        var playButtonObject = GameObject.Find("Play Button");
        var playButton = playButtonObject.GetComponent<Button>();
        playButton.onClick.Invoke();
        //2.
        var hostOptionPlayerName = GameObject.Find("NetworkOptionsClientName_inputField");
        var hostOptionPlayerNameText = hostOptionPlayerName.GetComponentInChildren<TMP_InputField>();
        hostOptionPlayerNameText.text = "dummy";
        Assert.AreEqual("dummy", hostOptionPlayerNameText.text);

        var startGameButtonObject = GameObject.Find("HostOption_directConnect");
        var startGameButton = startGameButtonObject.GetComponent<Button>();
        startGameButton.onClick.Invoke();
        yield return new WaitForSeconds(3);
    }

    //Start local match as host Test Scenario
    //1. Click Play Button
    //2. Start a game as host
    //3. Player spawned
    [UnityTest]
    public IEnumerator StartLocalMatchTest()
    {
        //1.
        var playButtonObject = GameObject.Find("Play Button");
        var playButton = playButtonObject.GetComponent<Button>();
        playButton.onClick.Invoke();
        //2.
        var hostOptionPlayerName = GameObject.Find("NetworkOptionsClientName_inputField");
        var hostOptionPlayerNameText = hostOptionPlayerName.GetComponentInChildren<TMP_InputField>();
        hostOptionPlayerNameText.text = "dummy";
        Assert.AreEqual("dummy", hostOptionPlayerNameText.text);

        var hostOptionHostName = GameObject.Find("HostOptionHostName_inputField");
        var hostOptionHostNameText = hostOptionHostName.GetComponentInChildren<TMP_InputField>();
        hostOptionHostNameText.text = "dummy";
        Assert.AreEqual("dummy", hostOptionHostNameText.text);

        var startGameButtonObject = GameObject.Find("HostOption_startHostButton");
        var startGameButton = startGameButtonObject.GetComponent<Button>();
        startGameButton.onClick.Invoke();
        yield return new WaitForSeconds(3);

        //3.
        var player = GameObject.Find("Player [connId=0]");
        Assert.IsNotNull(player);

        var playerName = GameObject.Find("Player_nameLabel");
        var PlayerNameText = playerName.GetComponent<TextMesh>().text;
        Assert.IsNotNull(PlayerNameText);
        Assert.AreEqual(PlayerNameText, hostOptionHostNameText.text);
    }

    //Join local match as client Test Scenario
    //1. Click Play Button
    //2. Join an already running game
    //3. Player spawned
    [UnityTest]
    public IEnumerator JoinLocalMatchTest()
    {
        //1.
        var playButtonObject = GameObject.Find("Play Button");
        var playButton = playButtonObject.GetComponent<Button>();
        playButton.onClick.Invoke();
        //2.
        var hostOptionPlayerName = GameObject.Find("NetworkOptionsClientName_inputField");
        var hostOptionPlayerNameText = hostOptionPlayerName.GetComponentInChildren<TMP_InputField>();
        hostOptionPlayerNameText.text = "dummy";
        Assert.AreEqual("dummy", hostOptionPlayerNameText.text);

        var hostOptionHostName = GameObject.Find("HostOptionHostName_");
        var hostOptionHostNameText = hostOptionHostName.GetComponentInChildren<TextMeshProUGUI>();
        hostOptionHostNameText.text = "dummy";
        Assert.AreEqual("dummy", hostOptionHostNameText.text);

        var ServerBrowserButtonObject = GameObject.Find("ServerBrowserButton(Clone)");
        var ServerBrowserButton = ServerBrowserButtonObject.GetComponent<Button>();
        ServerBrowserButton.onClick.Invoke();

        //3.
        var startGameButtonObject = GameObject.Find("ServerBrowserConnectButton");
        var startGameButton = startGameButtonObject.GetComponent<Button>();
        startGameButton.onClick.Invoke();
        yield return new WaitForSeconds(3);

        var player = GameObject.Find("Player(Clone)");
        Assert.IsNotNull(player);
    }

    //Menu scene UI Test Scenario
    //1. Click settings button
    //2. Check if settings displayed
    //3. Click on music and sfx button twice
    //4. Close settings tab
    //5. Click credits button
    //6. Check if credits displayed
    //7. Close credits
    [UnityTest]
    public IEnumerator MenuUITest()
    {
        //1.
        var settingButtonObject = GameObject.Find("Setting Button");
        var settingButton = settingButtonObject.GetComponent<Button>();
        settingButton.onClick.Invoke();

        //2.
        var settingPanel = GameObject.Find("Settings");
        Assert.NotNull(settingPanel);

        //3.
        var sfxButtonObject = GameObject.Find("SFX Button");
        var sfxButton = sfxButtonObject.GetComponent<Button>();
        sfxButton.onClick.Invoke();
        sfxButton.onClick.Invoke();
        var musicButtonObject = GameObject.Find("Music Button");
        var musicButton = musicButtonObject.GetComponent<Button>();
        musicButton.onClick.Invoke();
        musicButton.onClick.Invoke();

        //4.
        settingButton.onClick.Invoke();

        //5.
        var creditButtonObject = GameObject.Find("Credits Button");
        var creditButton = creditButtonObject.GetComponent<Button>();
        creditButton.onClick.Invoke();

        //6.
        var creditsPanel = GameObject.Find("Credits");
        Assert.NotNull(creditsPanel);

        //7.
        creditButton.onClick.Invoke();
        yield return null;
    }
}
