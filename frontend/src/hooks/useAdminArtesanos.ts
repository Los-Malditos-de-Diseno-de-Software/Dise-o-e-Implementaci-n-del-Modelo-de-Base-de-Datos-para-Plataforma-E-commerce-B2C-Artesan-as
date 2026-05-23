import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { artesanosApi } from '../api/artesanos.api';
import type { ArtesanoDto } from '../types';

export const useAdminArtesanos = () => {
  const queryClient = useQueryClient();

  const artesanosQuery = useQuery({
    queryKey: ['artesanos'],
    queryFn: () => artesanosApi.getArtesanos(),
  });

  const createMutation = useMutation({
    mutationFn: (params: Omit<ArtesanoDto, 'id'>) => artesanosApi.createArtesano(params),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['artesanos'] });
    },
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, params }: { id: string; params: Omit<ArtesanoDto, 'id'> }) =>
      artesanosApi.updateArtesano(id, params),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['artesanos'] });
      queryClient.invalidateQueries({ queryKey: ['artesano', variables.id] });
    },
  });

  const deleteMutation = useMutation({
    mutationFn: (id: string) => artesanosApi.deleteArtesano(id),
    onSuccess: (_, id) => {
      queryClient.invalidateQueries({ queryKey: ['artesanos'] });
      queryClient.invalidateQueries({ queryKey: ['artesano', id] });
    },
  });

  return {
    artesanos: artesanosQuery.data || [],
    isLoading: artesanosQuery.isLoading,
    createArtesano: createMutation.mutateAsync,
    isCreating: createMutation.isPending,
    updateArtesano: updateMutation.mutateAsync,
    isUpdating: updateMutation.isPending,
    deleteArtesano: deleteMutation.mutateAsync,
    isDeleting: deleteMutation.isPending,
  };
};
