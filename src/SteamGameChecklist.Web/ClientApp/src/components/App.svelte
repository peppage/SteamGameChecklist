<script>
	import { onMount } from "svelte";

	let games = [];
	let finished = 0;
	let total = 0;
	let ratio = 0;
	$: ratio = (finished / total) * 100;

	onMount(async () => {
	  const res = await fetch(`/api/checklist/allgames`);
	  games = await res.json();

	  const res2 = await fetch("/api/checklist/stats");
	  let data = await res2.json();
	  finished = data.finishedGames;
	  total = data.totalGames;
	});

	async function hideGame(game) {
	  const res = await fetch(`/api/checklist/hidegame`, {
	    method: "POST",
	    cache: "no-cache",
	    headers: {
	      "Content-Type": "application/json; charset=utf-8"
	    },
	    body: JSON.stringify({ gameId: game.id })
	  });

	  var status = await res.status;

	  if (status === 200) {
	    let index = games.indexOf(game);
	    games.splice(index, 1);
	    games = games;
	    finished += 1;
	  }
	}
</script>

<style>
</style>

<main>
	<div>
		<p>
			<b>Finished: </b> {finished}
		</p>
		<p>
			<b>Total: </b> {total}
		</p>
		<p>
			<b>Percent done: </b> {ratio}%
		</p>
	</div>
	<ul>
		{#each games as game}
			<li>
				<a href="https://store.steampowered.com/app/{game.id}">
					{game.name}
				</a>
				<div>
					Playtime last 2 weeks: {game.playtime2Weeks}
				</div>
				<div>
					Playtime total: {game.playtimeForever}
				</div>
				<div>
					<button on:click|preventDefault={() => hideGame(game)}>Hide</button>
				</div>
			</li>
		{/each}
	</ul>

</main>

