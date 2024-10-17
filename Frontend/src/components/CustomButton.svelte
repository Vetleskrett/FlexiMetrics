<script lang="ts">
	import { Button } from '$lib/components/ui/button';
	import * as Form from '$lib/components/ui/form';

	export let color: 'green' | 'blue' | 'yellow' | 'submit';
	export let href: string = '';
</script>

{#if color == 'submit'}
	<Form.Button
		{href}
		size="sm"
		class="flex flex-row justify-between gap-2 bg-button-green px-3 hover:bg-button-green-hover"
		on:click
		on:keydown
	>
		<slot />
	</Form.Button>
{:else if color == 'green'}
	<Button
		{href}
		size="sm"
		class="flex flex-row justify-between gap-2 bg-button-green px-3 hover:bg-button-green-hover"
		on:click
		on:keydown
	>
		<slot />
	</Button>
{:else if color == 'blue'}
	<Button
		{href}
		size="sm"
		class="flex flex-row justify-between gap-2 bg-button-blue px-3 hover:bg-button-blue-hover"
		on:click
		on:keydown
	>
		<slot />
	</Button>
{:else if color == 'yellow'}
	<Button
		{href}
		size="sm"
		class="flex flex-row justify-between gap-2 bg-button-yellow px-3 hover:bg-button-yellow-hover"
		on:click
		on:keydown
	>
		<slot />
	</Button>
{/if}
