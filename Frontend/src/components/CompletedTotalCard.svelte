<script lang="ts">
	import * as Card from '$lib/components/ui/card';

	export let completed: number;
	export let total: number;
	export let headline: string;
</script>

<Card.Root class="w-full overflow-hidden p-5">
	<Card.Header class="m-0 p-0">
		<div class="flex items-center">
			<img width="40" height="40" src="https://img.icons8.com/fluency/480/pass.png" alt="pass" />
			<Card.Title class="m-0 ml-2 text-2xl">{headline}</Card.Title>
		</div>
	</Card.Header>
	<Card.Content class="p-0 py-8">
		<h1 class="m-auto text-center text-5xl font-semibold">{completed} / {total}</h1>
	</Card.Content>
</Card.Root>
