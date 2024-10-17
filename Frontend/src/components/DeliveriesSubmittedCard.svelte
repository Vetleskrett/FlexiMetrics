<script lang="ts">
	import * as Card from '$lib/components/ui/card/index.js';

	export let deliveriesSubmitted: number;
	export let numTeams: number;
</script>

<Card.Root class="w-full overflow-hidden p-5">
	<Card.Header class="m-0 p-0">
		<div class="flex items-center">
			<img
				width="40"
				height="40"
				src="https://img.icons8.com/fluency/480/pass.png"
				alt="knowledge-sharing"
			/>
			<Card.Title class="m-0 ml-2 text-2xl">Deliveries Submitted</Card.Title>
		</div>
	</Card.Header>
	<Card.Content class="p-0 py-8">
		<h1 class="m-auto text-center text-5xl font-semibold">{deliveriesSubmitted} / {numTeams}</h1>
	</Card.Content>
</Card.Root>
