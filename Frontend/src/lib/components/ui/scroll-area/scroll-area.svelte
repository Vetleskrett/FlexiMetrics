<script lang="ts">
	import { ScrollArea as ScrollAreaPrimitive } from 'bits-ui';
	import { Scrollbar } from './index.js';
	import { cn } from '$lib/utils.js';

	type $$Props = ScrollAreaPrimitive.Props & {
		orientation?: 'vertical' | 'horizontal' | 'both';
		position?: 'before' | 'after' | 'both';
		scrollbarXClasses?: string;
		scrollbarYClasses?: string;
	};

	let className: $$Props['class'] = undefined;
	export { className as class };
	export let orientation = 'vertical';
	export let position = 'after';
	export let scrollbarXClasses: string = '';
	export let scrollbarYClasses: string = '';
</script>

<ScrollAreaPrimitive.Root {...$$restProps} class={cn('relative overflow-hidden', className)}>
	<ScrollAreaPrimitive.Viewport class="h-full w-full rounded-[inherit]">
		<ScrollAreaPrimitive.Content>
			<slot />
		</ScrollAreaPrimitive.Content>
	</ScrollAreaPrimitive.Viewport>
	{#if position == 'before' || position == 'both'}
		{#if orientation === 'vertical' || orientation === 'both'}
			<Scrollbar orientation="vertical" position="before" class={scrollbarYClasses} />
		{/if}
		{#if orientation === 'horizontal' || orientation === 'both'}
			<Scrollbar orientation="horizontal" position="before" class={scrollbarXClasses} />
		{/if}
	{/if}
	{#if position == 'after' || position == 'both'}
		{#if orientation === 'vertical' || orientation === 'both'}
			<Scrollbar orientation="vertical" position="after" class={scrollbarYClasses} />
		{/if}
		{#if orientation === 'horizontal' || orientation === 'both'}
			<Scrollbar orientation="horizontal" position="after" class={scrollbarXClasses} />
		{/if}
	{/if}
	<ScrollAreaPrimitive.Corner />
</ScrollAreaPrimitive.Root>
