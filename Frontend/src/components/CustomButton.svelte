<script lang="ts">
	import { Button } from '$lib/components/ui/button';
	import type { Builder } from 'bits-ui';
	import * as Form from 'src/lib/components/ui/form';

	export let color: 'red' | 'green' | 'blue' | 'yellow' | 'submit';
	export let href: string = '';
	export let outline: boolean = false;
	export let disabled: boolean = false;

	export let builders: Builder[] | undefined = undefined;
</script>

{#if outline}
	{#if color == 'red'}
		<Button
			{builders}
			{href}
			variant="outline"
			size="sm"
			class="flex flex-row justify-between gap-2 rounded-sm border-button-red bg-white px-3 text-button-red hover:border-button-red-hover hover:bg-red-50 hover:text-button-red-hover"
			on:click
			on:keydown
			disabled={disabled}
		>
			<slot />
		</Button>
	{:else if color == 'green'}
		<Button
			{builders}
			{href}
			variant="outline"
			size="sm"
			class="flex flex-row justify-between gap-2 rounded-sm border-button-green bg-white px-3 text-button-green hover:border-button-green-hover hover:bg-green-50 hover:text-button-green-hover"
			on:click
			on:keydown
			disabled={disabled}
		>
			<slot />
		</Button>
	{:else if color == 'blue'}
		<Button
			{builders}
			{href}
			variant="outline"
			size="sm"
			class="flex flex-row justify-between gap-2 rounded-sm border-button-blue bg-white px-3 text-button-blue hover:border-button-blue-hover hover:bg-blue-50 hover:text-button-blue-hover"
			on:click
			on:keydown
			disabled={disabled}
		>
			<slot />
		</Button>
	{:else if color == 'yellow'}
		<Button
			{builders}
			{href}
			variant="outline"
			size="sm"
			class="flex flex-row justify-between gap-2 rounded-sm border-button-yellow bg-white px-3 text-button-yellow hover:border-button-yellow-hover hover:bg-yellow-50 hover:text-button-yellow-hover"
			on:click
			on:keydown
			disabled={disabled}
		>
			<slot />
		</Button>
	{:else if color == 'submit'}
		<Form.Button
			{builders}
			{href}
			variant="outline"
			size="sm"
			class="flex flex-row justify-between gap-2 rounded-sm border-button-green bg-white px-3 text-button-green hover:border-button-green-hover hover:bg-green-50 hover:text-button-green-hover"
			on:click
			on:keydown
			disabled={disabled}
		>
			<slot />
		</Form.Button>
	{/if}
{:else if !outline}
	{#if color == 'red'}
		<Button
			{builders}
			{href}
			size="sm"
			class="flex flex-row justify-between gap-2 rounded-sm bg-button-red px-3 hover:bg-button-red-hover"
			on:click
			on:keydown
			disabled={disabled}
		>
			<slot />
		</Button>
	{:else if color == 'green'}
		<Button
			{builders}
			{href}
			size="sm"
			class="flex flex-row justify-between gap-2 rounded-sm bg-button-green px-3 hover:bg-button-green-hover"
			on:click
			on:keydown
			disabled={disabled}
		>
			<slot />
		</Button>
	{:else if color == 'blue'}
		<Button
			{builders}
			{href}
			size="sm"
			class="flex flex-row justify-between gap-2 rounded-sm bg-button-blue px-3 hover:bg-button-blue-hover"
			on:click
			on:keydown
			disabled={disabled}
		>
			<slot />
		</Button>
	{:else if color == 'yellow'}
		<Button
			{builders}
			{href}
			size="sm"
			class="flex flex-row justify-between gap-2 rounded-sm bg-button-yellow px-3 hover:bg-button-yellow-hover"
			on:click
			on:keydown
			disabled={disabled}
		>
			<slot />
		</Button>
	{:else if color == 'submit'}
		<Form.Button
			{builders}
			{href}
			size="sm"
			class="flex flex-row justify-between gap-2 rounded-sm bg-button-green px-3 hover:bg-button-green-hover"
			on:click
			on:keydown
			disabled={disabled}
		>
			<slot />
		</Form.Button>
	{/if}
{/if}
