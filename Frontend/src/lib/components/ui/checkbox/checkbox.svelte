<script lang="ts">
	import { Checkbox as CheckboxPrimitive } from 'bits-ui';
	import Check from 'lucide-svelte/icons/check';
	import Minus from 'lucide-svelte/icons/minus';
	import { cn } from '$lib/utils.js';

	type $$Props = CheckboxPrimitive.Props;
	type $$Events = CheckboxPrimitive.Events;

	let className: $$Props['class'] = undefined;
	export let checked: $$Props['checked'] = false;
	export { className as class };
</script>

<CheckboxPrimitive.Root
	class={cn(
		'peer box-content h-6 w-6 shrink-0 rounded border border-primary ring-offset-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50 data-[disabled=true]:cursor-not-allowed data-[state=checked]:bg-primary data-[state=checked]:text-primary-foreground data-[disabled=true]:opacity-50',
		className,
		'border-gray-400'
	)}
	bind:checked
	{...$$restProps}
	on:click
>
	<CheckboxPrimitive.Indicator
		class={cn('flex h-full w-full items-center justify-center p-0.5 text-current')}
		let:isChecked
		let:isIndeterminate
	>
		{#if isChecked}
			<Check />
		{:else if isIndeterminate}
			<Minus />
		{/if}
	</CheckboxPrimitive.Indicator>
</CheckboxPrimitive.Root>
