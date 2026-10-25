(() => {
	const retryIntervalMs = 10000;
	const status = document.getElementById('connection-status');
	let retryTimer = null;

	const serverIsReachable = async () => {
		try {
			const response = await fetch('/health', { cache: 'no-store' });
			return response.ok;
		} catch {
			return false;
		}
	};

	const reloadWhenServerIsBack = async () => {
		if (await serverIsReachable())
			location.reload();
	};

	const onConnectionDown = () => {
		status.hidden = false;
		retryTimer ??= setInterval(reloadWhenServerIsBack, retryIntervalMs);
	};

	const onConnectionUp = () => {
		status.hidden = true;
		clearInterval(retryTimer);
		retryTimer = null;
	};

	Blazor.start({
		circuit: {
			reconnectionHandler: { onConnectionDown, onConnectionUp }
		}
	});
})();
