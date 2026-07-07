import { useState } from 'react'
import { IconCheck, IconPencil, IconPlus, IconTrash } from '@tabler/icons-react'
import { Modal } from '@/components/ui/Modal'
import { Button } from '@/components/ui/Button'
import { Input } from '@/components/ui/Input'
import { CategoryIcon, CATEGORY_ICON_CHOICES } from '@/components/CategoryIcon'
import { useToast } from '@/context/ToastContext'
import {
  useBudgetCategories, useCreateBudgetCategory, useUpdateBudgetCategory, useDeleteBudgetCategory,
} from '@/hooks/useBudgetCategories'
import { CategoryType, type BudgetCategory } from '@/types/models'

const COLOR_CHOICES = [
  '#4f46e5', '#22c55e', '#f59e0b', '#ef4444', '#0ea5e9', '#a855f7', '#ec4899', '#14b8a6', '#64748b',
]

interface DraftCategory {
  Id?: string
  Name: string
  Icon: string
  Color: string
  CategoryType: CategoryType
}

const emptyDraft: DraftCategory = {
  Name: '',
  Icon: CATEGORY_ICON_CHOICES[0],
  Color: COLOR_CHOICES[0],
  CategoryType: CategoryType.Expense,
}

export function CategoryManagerModal({ open, onClose, sharedBudgetId }: { open: boolean; onClose: () => void; sharedBudgetId: string }) {
  const { data: categories } = useBudgetCategories(sharedBudgetId)
  const createCategory = useCreateBudgetCategory()
  const updateCategory = useUpdateBudgetCategory()
  const deleteCategory = useDeleteBudgetCategory()
  const { show } = useToast()

  const [draft, setDraft] = useState<DraftCategory | null>(null)

  const startCreate = () => setDraft({ ...emptyDraft })
  const startEdit = (c: BudgetCategory) =>
    setDraft({ Id: c.Id, Name: c.Name, Icon: c.Icon, Color: c.Color, CategoryType: c.CategoryType })

  const save = async () => {
    if (!draft || !draft.Name.trim()) return
    try {
      if (draft.Id) {
        const existing = categories?.find((c) => c.Id === draft.Id)
        await updateCategory.mutateAsync({
          id: draft.Id,
          category: {
            ...existing!,
            Name: draft.Name,
            Icon: draft.Icon,
            Color: draft.Color,
            CategoryType: draft.CategoryType,
          },
        })
      } else {
        await createCategory.mutateAsync({
          SharedBudgetId: sharedBudgetId,
          Name: draft.Name,
          Icon: draft.Icon,
          Color: draft.Color,
          CategoryType: draft.CategoryType,
          SortOrder: categories?.length ?? 0,
          IsActive: true,
        })
      }
      show('Category saved', 'success')
      setDraft(null)
    } catch {
      show('Could not save category', 'error')
    }
  }

  const remove = async (id: string) => {
    try {
      await deleteCategory.mutateAsync(id)
      show('Category deleted', 'success')
    } catch {
      show('Could not delete — it may have existing transactions.', 'error')
    }
  }

  return (
    <Modal open={open} onClose={onClose} title="Manage categories">
      {draft ? (
        <div className="flex flex-col gap-4">
          <Input label="Name" value={draft.Name} onChange={(e) => setDraft({ ...draft, Name: e.target.value })} autoFocus />

          <div>
            <p className="mb-1.5 text-sm font-medium text-[var(--text-secondary)]">Type</p>
            <div className="flex flex-wrap gap-2">
              {(['Income', 'Expense', 'Savings', 'Transfer'] as const).map((label, idx) => (
                <button
                  key={label}
                  onClick={() => setDraft({ ...draft, CategoryType: idx as CategoryType })}
                  className={`rounded-lg px-3 py-1.5 text-sm font-medium transition-colors ${
                    draft.CategoryType === idx
                      ? 'bg-brand-600 text-white'
                      : 'bg-[var(--surface-sunken)] text-[var(--text-secondary)]'
                  }`}
                >
                  {label}
                </button>
              ))}
            </div>
          </div>

          <div>
            <p className="mb-1.5 text-sm font-medium text-[var(--text-secondary)]">Icon</p>
            <div className="grid grid-cols-8 gap-2">
              {CATEGORY_ICON_CHOICES.map((icon) => (
                <button
                  key={icon}
                  onClick={() => setDraft({ ...draft, Icon: icon })}
                  className={`flex h-9 w-9 items-center justify-center rounded-lg transition-colors ${
                    draft.Icon === icon ? 'bg-brand-600 text-white' : 'bg-[var(--surface-sunken)] text-[var(--text-secondary)]'
                  }`}
                >
                  <CategoryIcon icon={icon} size={17} />
                </button>
              ))}
            </div>
          </div>

          <div>
            <p className="mb-1.5 text-sm font-medium text-[var(--text-secondary)]">Color</p>
            <div className="flex flex-wrap gap-2">
              {COLOR_CHOICES.map((color) => (
                <button
                  key={color}
                  onClick={() => setDraft({ ...draft, Color: color })}
                  className="flex h-8 w-8 items-center justify-center rounded-full ring-offset-2 ring-offset-[var(--surface-raised)]"
                  style={{ backgroundColor: color, boxShadow: draft.Color === color ? `0 0 0 2px ${color}` : undefined }}
                >
                  {draft.Color === color && <IconCheck size={15} className="text-white" />}
                </button>
              ))}
            </div>
          </div>

          <div className="flex gap-2">
            <Button variant="secondary" fullWidth onClick={() => setDraft(null)}>
              Cancel
            </Button>
            <Button fullWidth onClick={save} loading={createCategory.isPending || updateCategory.isPending}>
              Save
            </Button>
          </div>
        </div>
      ) : (
        <div className="flex flex-col gap-4">
          <div className="flex max-h-80 flex-col gap-1 overflow-y-auto">
            {categories?.map((c) => (
              <div key={c.Id} className="flex items-center gap-2 rounded-xl px-2 py-2 hover:bg-[var(--surface-sunken)]">
                <div
                  className="flex h-8 w-8 shrink-0 items-center justify-center rounded-full"
                  style={{ backgroundColor: `${c.Color}22`, color: c.Color }}
                >
                  <CategoryIcon icon={c.Icon} size={16} />
                </div>
                <span className="flex-1 truncate text-sm font-medium text-[var(--text-primary)]">{c.Name}</span>
                <button onClick={() => startEdit(c)} className="rounded-lg p-1.5 text-[var(--text-muted)] hover:text-brand-600">
                  <IconPencil size={16} />
                </button>
                <button onClick={() => remove(c.Id)} className="rounded-lg p-1.5 text-[var(--text-muted)] hover:text-red-500">
                  <IconTrash size={16} />
                </button>
              </div>
            ))}
          </div>
          <Button variant="secondary" fullWidth onClick={startCreate}>
            <IconPlus size={16} /> Add category
          </Button>
        </div>
      )}
    </Modal>
  )
}
