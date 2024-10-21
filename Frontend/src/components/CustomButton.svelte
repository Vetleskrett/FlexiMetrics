<script lang="ts">
	import { Button } from '$lib/components/ui/button';
	import * as Form from '$lib/components/ui/form';
	import type { Builder } from 'bits-ui';

	export let color: 'red' | 'green' | 'blue' | 'yellow' | 'submit' | 'outline';
	export let href: string = '';

	export let builders: Builder[] | undefined = undefined;
</script>

{#if color == 'submit'}
	<Form.Button
		{builders}
		{href}
		size="sm"
		class="flex flex-row justify-between gap-2 bg-button-green px-3 hover:bg-button-green-hover"
		on:click
		on:keydown
	>
		<slot />
	</Form.Button>
{:else if color == 'outline'}
	<Button
		{builders}
		{href}
		variant="outline"
		size="sm"
		class="flex flex-row justify-between gap-2 border-button-blue bg-white px-3 text-button-blue hover:border-button-blue-hover hover:bg-blue-50 hover:text-button-blue-hover"
		on:click
		on:keydown
	>
		<slot />
	</Button>
{:else if color == 'red'}
	<Button
		{builders}
		{href}
		size="sm"
		class="flex flex-row justify-between gap-2 bg-button-red px-3 hover:bg-button-red-hover"
		on:click
		on:keydown
	>
		<slot />
	</Button>
{:else if color == 'green'}
	<Button
		{builders}
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
		{builders}
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
		{builders}
		{href}
		size="sm"
		class="flex flex-row justify-between gap-2 bg-button-yellow px-3 hover:bg-button-yellow-hover"
		on:click
		on:keydown
	>
		<slot />
	</Button>
{/if}
