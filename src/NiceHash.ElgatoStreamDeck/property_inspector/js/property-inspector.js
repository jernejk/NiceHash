// global websocket, used to communicate from/to Stream Deck software
// as well as some info about our plugin, as sent by Stream Deck software 
var websocket = null,
  uuid = null,
  inInfo = null,
  actionInfo = {},
  settingsModel = {
	  BaseUrl: 'https://api2.nicehash.com',
	  OrganizationId: '',
	  ApiKey: '',
	  ApiSecret: '',
	  MainCurrency: 'USD',
	  FreeCurrencyApiKey: '',
	  UpdateInterval: 60
  };

function connectElgatoStreamDeckSocket(inPort, inUUID, inRegisterEvent, inInfo, inActionInfo) {
	uuid = inUUID;
	actionInfo = JSON.parse(inActionInfo);
	inInfo = JSON.parse(inInfo);
	websocket = new WebSocket('ws://localhost:' + inPort);

	//initialize values
	if (actionInfo.payload.settings.settingsModel) {
		settingsModel.BaseUrl = actionInfo.payload.settings.settingsModel.BaseUrl;
		settingsModel.OrganizationId = actionInfo.payload.settings.settingsModel.OrganizationId;
		settingsModel.ApiKey = actionInfo.payload.settings.settingsModel.ApiKey;
		settingsModel.ApiSecret = actionInfo.payload.settings.settingsModel.ApiSecret;
		settingsModel.MainCurrency = actionInfo.payload.settings.settingsModel.MainCurrency;
		settingsModel.FreeCurrencyApiKey = actionInfo.payload.settings.settingsModel.FreeCurrencyApiKey;
		settingsModel.UpdateInterval = actionInfo.payload.settings.settingsModel.UpdateInterval;
	} else {
		settingsModel.BaseUrl = 'https://api2.nicehash.com';
		settingsModel.MainCurrency = 'USD';
		settingsModel.UpdateInterval = 60;
    }

	document.getElementById('txtNiceHashUrl').value = settingsModel.BaseUrl;
	document.getElementById('txtOrganizationId').value = settingsModel.OrganizationId;
	document.getElementById('txtApiKey').value = settingsModel.ApiKey;
	document.getElementById('txtApiSecret').value = settingsModel.ApiSecret;
	document.getElementById('txtMainCurrency').value = settingsModel.MainCurrency;
	document.getElementById('txtFreeCurrencyApiKey').value = settingsModel.FreeCurrencyApiKey;
	document.getElementById('update_interval').value = settingsModel.UpdateInterval;

	websocket.onopen = function () {
		var json = { event: inRegisterEvent, uuid: inUUID };
		// register property inspector to Stream Deck
		websocket.send(JSON.stringify(json));

	};

	websocket.onmessage = function (evt) {
		// Received message from Stream Deck
		var jsonObj = JSON.parse(evt.data);
		var sdEvent = jsonObj['event'];
		switch (sdEvent) {
			case "didReceiveSettings":
				if (jsonObj.payload.settings.settingsModel.BaseUrl) {
					settingsModel.BaseUrl = jsonObj.payload.settings.settingsModel.BaseUrl;
					document.getElementById('txtNiceHashUrl').value = settingsModel.BaseUrl;
				}

				if (jsonObj.payload.settings.settingsModel.OrganizationId) {
					settingsModel.OrganizationId = jsonObj.payload.settings.settingsModel.OrganizationId;
					document.getElementById('txtOrganizationId').value = settingsModel.OrganizationId;
				}

				if (jsonObj.payload.settings.settingsModel.ApiKey) {
					settingsModel.ApiKey = jsonObj.payload.settings.settingsModel.ApiKey;
					document.getElementById('txtApiKey').value = settingsModel.ApiKey;
				}

				if (jsonObj.payload.settings.settingsModel.ApiSecret) {
					settingsModel.ApiSecret = jsonObj.payload.settings.settingsModel.ApiSecret;
					document.getElementById('txtApiSecret').value = settingsModel.ApiSecret;
				}

				if (jsonObj.payload.settings.settingsModel.MainCurrency) {
					settingsModel.MainCurrency = jsonObj.payload.settings.settingsModel.MainCurrency;
					document.getElementById('txtMainCurrency').value = settingsModel.MainCurrency;
				}

				if (jsonObj.payload.settings.settingsModel.FreeCurrencyApiKey) {
					settingsModel.FreeCurrencyApiKey = jsonObj.payload.settings.settingsModel.FreeCurrencyApiKey;
					document.getElementById('txtFreeCurrencyApiKey').value = settingsModel.FreeCurrencyApiKey;
				}

				if (jsonObj.payload.settings.settingsModel.UpdateInterval) {
					settingsModel.UpdateInterval = jsonObj.payload.settings.settingsModel.UpdateInterval;
					document.getElementById('update_interval').value = settingsModel.UpdateInterval;
				}

				break;
			default:
				break;
		}
	};
}

const setSettings = (value, param) => {
	if (websocket) {
		settingsModel[param] = value;
		var json = {
			"event": "setSettings",
			"context": uuid,
			"payload": {
				"settingsModel": settingsModel
			}
		};
		websocket.send(JSON.stringify(json));
	}
};

