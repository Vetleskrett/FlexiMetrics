<script lang="ts">
	import { type AssignmentField, type Delivery, type DeliveryField } from 'src/types.js';
	import * as Card from '$lib/components/ui/card/index.js';
	import { Separator } from '$lib/components/ui/separator/index.js';
	import { getDeliveryFieldFile } from 'src/api';
	import ArrowDownToLine from 'lucide-svelte/icons/arrow-down-to-line';

	export let assignmentFields: AssignmentField[];
	export let delivery: Delivery | null;
</script>

<Card.Root class="w-full overflow-hidden p-0">
	<Card.Header class="mb-6 flex flex-row items-center justify-between">
		<div class="flex items-center">
			<img
				width="40"
				height="40"
				src="https://img.icons8.com/fluency/480/pass.png"
				alt="knowledge-sharing"
			/>
			<Card.Title class="m-0 ml-2 text-2xl">Delivery</Card.Title>
		</div>
	</Card.Header>
	<Card.Content class="p-0">
		<div class="flex flex-col">
			{#if delivery}
				{#each assignmentFields as assignmentField}
				{@const deliveryField = delivery.fields.find(f => f.assignmentFieldId == assignmentField.id)}
					<Separator class="w-full" />
					<div class="px-6 py-4">
						<h1 class="font-semibold">
							{assignmentField.name}
						</h1>
						{#if assignmentField.type == 'URL'}
							<a class="text-blue-500" target="_blank" href={deliveryField?.value}>{deliveryField?.value}</a>
						{:else if assignmentField.type == 'File'}
							<a class="flex items-center text-blue-500" download href={getDeliveryFieldFile(deliveryField?.id || '')}>
								<ArrowDownToLine size="20" />
								{deliveryField?.value?.FileName}
							</a>
						{:else}
							<h1>{deliveryField?.value}</h1>
						{/if}
					</div>
				{/each}
			{:else}
				<p class="mx-auto mb-4">Not delivered</p>
			{/if}
		</div>
	</Card.Content>
</Card.Root>
