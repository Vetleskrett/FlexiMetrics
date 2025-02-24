<script lang="ts">
	import type { AssignmentField, Delivery } from 'src/types.js';
	import * as Card from '$lib/components/ui/card/index.js';
	import { Separator } from '$lib/components/ui/separator/index.js';
	import DeliveryFieldValue from './DeliveryFieldValue.svelte';

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
					{@const deliveryField = delivery.fields.find(
						(f) => f.assignmentFieldId == assignmentField.id
					)}
					<Separator class="w-full" />
					<div class="px-6 py-4">
						<h1 class="font-semibold">
							{assignmentField.name}
						</h1>
						{#if deliveryField}
							<DeliveryFieldValue
								id={deliveryField.id}
								value={deliveryField.value}
								type={assignmentField.type}
								subType={assignmentField.subType}
							/>
						{/if}
					</div>
				{/each}
			{:else}
				<p class="mx-auto mb-4">Not delivered</p>
			{/if}
		</div>
	</Card.Content>
</Card.Root>
