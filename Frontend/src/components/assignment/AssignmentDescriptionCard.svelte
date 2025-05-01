<script lang="ts">
	import { type Assignment } from 'src/types/';
	import * as Card from '$lib/components/ui/card';

	export let assignment: Assignment;
</script>

<Card.Root class="w-full overflow-hidden p-0">
	<Card.Header class="flex flex-row items-center justify-between">
		<div class="flex items-center">
			<img
				width="40"
				height="40"
				src="https://img.icons8.com/fluency/480/speech-bubble.png"
				alt="knowledge-sharing"
			/>
			<Card.Title class="m-0 ml-2 text-2xl">Description</Card.Title>
		</div>
	</Card.Header>
	<Card.Content class="py-6" style="white-space: pre-wrap">
		<p>{assignment?.description}</p>
	</Card.Content>
</Card.Root>
